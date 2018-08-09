using System;
using Microsoft.Extensions.Options;
using Novell.Directory.Ldap;
using LdapRemoteCertificateValidationCallback =
        Novell.Directory.Ldap.RemoteCertificateValidationCallback;
using Unosquare.PassCore.Common;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using Microsoft.Extensions.Logging;

namespace Zyborg.PassCore.PasswordProvider.LDAP
{
    public class LdapPasswordChangeProvider : IPasswordChangeProvider
    {
        private ILogger _logger;
        private LdapPasswordChangeOptions _options;
        private LdapRemoteCertificateValidationCallback _ldapRemoteCertValidator = null;

        public LdapPasswordChangeProvider(ILogger<LdapPasswordChangeProvider> logger,
            IOptions<LdapPasswordChangeOptions> options)
        {
            _logger = logger;
            _options = options.Value;

            Init();
        }

        private void Init()
        {
            if (_options.LdapIgnoreTlsErrors || _options.LdapIgnoreTlsValidation)
                _ldapRemoteCertValidator = CustomServerCertValidation;
        }

        public ApiErrorItem PerformPasswordChange(string username,
                string currentPassword, string newPassword)
        {
            var cleanUsername = username;
            // Must sanitize the username to eliminate the possibility of injection attacks:
            //    * https://docs.microsoft.com/en-us/windows/desktop/adschema/a-samaccountname
            //    * https://docs.microsoft.com/en-us/previous-versions/windows/it-pro/windows-2000-server/bb726984(v=technet.10)
            var samInvalid = "\"/\\[]:;|=,+*?<>";
            var miscInvalid = "\r\n\t";
            var invalid = (samInvalid + miscInvalid).ToCharArray();
            var invalidIndex = username.IndexOfAny(invalid);
            if (invalidIndex >= 0)
            {
                var msg = "username contains one or more invalid characters";
                _logger.LogWarning(msg);
                return new ApiErrorItem
                {
                    ErrorCode = ApiErrorCode.InvalidCredentials,
                    FieldName = nameof(username),
                    Message = msg,
                };
            }
            
            // LDAP filters require escaping of some special chars:
            //    * http://www.ldapexplorer.com/en/manual/109010000-ldap-filter-syntax.htm
            var escape = "()&|=><!*/\\".ToCharArray();
            var escapeIndex = cleanUsername.IndexOfAny(escape);
            if (escapeIndex >= 0)
            {
                var buff = new StringBuilder();
                var maxLen = cleanUsername.Length;
                var copyFrom = 0;
                while (escapeIndex >= 0)
                {
                    buff.Append(cleanUsername.Substring(copyFrom, escapeIndex));
                    buff.Append(string.Format("\\{0:X}", (int)cleanUsername[escapeIndex]));
                    copyFrom = escapeIndex + 1;
                    escapeIndex = cleanUsername.IndexOfAny(escape, copyFrom);
                }
                if (copyFrom < maxLen)
                    buff.Append(cleanUsername.Substring(copyFrom));
                cleanUsername = buff.ToString();
                _logger.LogWarning("had to clean username: [{0}] => [{1}]", username, cleanUsername);
            }

            // Based on:
            //    * https://www.cs.bham.ac.uk/~smp/resources/ad-passwds/
            //    * https://support.microsoft.com/en-us/help/269190/how-to-change-a-windows-active-directory-and-lds-user-password-through
            //    * https://ltb-project.org/documentation/self-service-password/latest/config_ldap#active_directory
            //    * https://technet.microsoft.com/en-us/library/ff848710.aspx?f=255&MSPPError=-2147217396

            using (var ldap = BindToLdap())
            {
                // First find user DN by username (SAM Account Name)
                var searchConstraints = new LdapSearchConstraints(
                        0, 0, LdapSearchConstraints.DEREF_NEVER,
                        1000, true, 1, null, 10);
                
                var searchFilter = $"(sAMAccountName={cleanUsername})";
                var search = ldap.Search(
                        _options.LdapSearchBase, LdapConnection.SCOPE_SUB,
                        searchFilter, new[] { "distinguishedName" },
                        false, searchConstraints);
                
                if (search.Count < 1)
                {
                    _logger.LogWarning("unable to find username: [{0}]", cleanUsername);
                    if (_options.HideUserNotFound)
                    {
                        return new ApiErrorItem
                        {
                            ErrorCode = ApiErrorCode.InvalidCredentials,
                            FieldName = nameof(currentPassword),
                            Message = "invalid credentials",
                        };
                    }
                    else
                    {
                        return new ApiErrorItem
                        {
                            ErrorCode = ApiErrorCode.UserNotFound,
                            FieldName = nameof(username),
                            Message = "username could not be located",
                        };
                    }
                }

                if (search.Count > 1)
                {
                    _logger.LogWarning("found multipel with same username: [{0}]", cleanUsername);
                    // Hopefully this should not ever happen if AD is preserving SAM Account Name
                    // uniqueness constraint, but just in case, handling this corner case
                    return new ApiErrorItem
                    {
                        ErrorCode = ApiErrorCode.UserNotFound,
                        FieldName = nameof(username),
                        Message = "multiple matching user entries resolved",
                    };
                }

                var userDN = search.next().DN;

                var oldPassBytes = Encoding.Unicode.GetBytes($@"""{currentPassword}""")
                        .Select(x => (sbyte)x).ToArray();
                var newPassBytes = Encoding.Unicode.GetBytes($@"""{newPassword}""")
                        .Select(x => (sbyte)x).ToArray();
                
                var oldAttr = new LdapAttribute("unicodePwd", oldPassBytes);
                var newAttr = new LdapAttribute("unicodePwd", newPassBytes);

                var ldapDel = new LdapModification(LdapModification.DELETE, oldAttr);
                var ldapAdd = new LdapModification(LdapModification.ADD, newAttr);

                try
                {
                    ldap.Modify(userDN, new[] { ldapDel, ldapAdd });
                }
                catch (LdapException ex)
                {
                    _logger.LogWarning("failed to update password", ex);
                    return ParseLdapException(ex);
                }

                ldap.StopTls();
                ldap.Disconnect();
            }

            // Everything seems to have worked:
            return null;
        }

        private ApiErrorItem ParseLdapException(LdapException ex)
        {
            // If the LDAP server returned an error, it will be formated
            // similar to this:
            //    "0000052D: AtrErr: DSID-03191083, #1:\n\t0: 0000052D: DSID-03191083, problem 1005 (CONSTRAINT_ATT_TYPE), data 0, Att 9005a (unicodePwd)\n\0"
            //
            // The leading number before the ':' is the Win32 API Error Code in HEX
            var m = Regex.Match(ex.LdapErrorMessage, "([0-9a-fA-F]+):");
            if (m.Success)
            {
                var errCodeString = m.Groups[1].Value;
                var errCode = int.Parse(errCodeString, NumberStyles.HexNumber);
                var err = Win32ErrorCode.ByCode(errCode);

                if (err != null)
                {
                    _logger.LogWarning("resolved Win32 API Error: code={0} name={1} desc={2}",
                            err.Code, err.CodeName, err.Description);
                    return new ApiErrorItem
                    {
                        ErrorCode = ApiErrorCode.InvalidCredentials,
                        FieldName = "currentPassword",
                        Message = $"0x{err.Code:X}:{err.CodeName}: {err.Description}",
                    };
                }

                return new ApiErrorItem
                {
                    ErrorCode = ApiErrorCode.Generic,
                    Message = "unexpected Win32 API error; error code: " + errCodeString,
                };
            }
            return new ApiErrorItem
            {
                ErrorCode = ApiErrorCode.Generic,
                Message = "unexpected error: " + ex.LdapErrorMessage,
            };
        }

        private LdapConnection BindToLdap()
        {
            var ldap = new LdapConnection();
            if (_ldapRemoteCertValidator != null)
                ldap.UserDefinedServerCertValidationDelegate += _ldapRemoteCertValidator;

            if (!_options.LdapStartTls)
                ldap.SecureSocketLayer = true;

            string bindHostname = null;
            foreach (var h in _options.LdapHostnames)
            {
                try
                {
                    ldap.Connect(h, _options.LdapPort);
                    bindHostname = h;
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning("failed to log to host: " + h, ex);
                }
            }

            if (string.IsNullOrEmpty(bindHostname))
                throw new Exception("failed to connect to any specified hostname");
            
            if (_options.LdapStartTls)
                ldap.StartTls();
            
            ldap.Bind(_options.LdapBindUserDN, _options.LdapBindPassword);

            return ldap;            
        }

        /// Custom server certificate validation logic that handles our special
        /// cases based on configuration.  This implements the logic of supporting
        /// custom CA-certs, custom CA thumbprint matching and also just ignoring
        /// server certificate validation altogether.
        private bool CustomServerCertValidation(object sender, X509Certificate certificate,
                X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (_options.LdapIgnoreTlsErrors || sslPolicyErrors == SslPolicyErrors.None)
                return true;

            var errorStatuses = chain.ChainStatus.Select((x, y) => (status: x, index: y)).Where(x => {
                if (x.status.Status == X509ChainStatusFlags.UntrustedRoot
                        && _options.LdapIgnoreTlsValidation)
                    return false;
                if (x.status.Status == X509ChainStatusFlags.NoError)
                    return false;
                return true;
            }).ToArray();

            return errorStatuses.Length > 0;
        }
    }
}
