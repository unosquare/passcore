﻿namespace Zyborg.PassCore.PasswordProvider.LDAP
{
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Novell.Directory.Ldap;
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Net.Security;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using System.Text.RegularExpressions;
    using Unosquare.PassCore.Common;
    using LdapRemoteCertificateValidationCallback =
        Novell.Directory.Ldap.RemoteCertificateValidationCallback;

    /// <summary>
    /// Represents a LDAP password change provider using Novell LDAP Connection.
    /// </summary>
    /// <seealso cref="Unosquare.PassCore.Common.IPasswordChangeProvider" />
    public class LdapPasswordChangeProvider : IPasswordChangeProvider
    {
        private readonly LdapPasswordChangeOptions _options;
        private readonly ILogger _logger;

        // First find user DN by username (SAM Account Name)
        private readonly LdapSearchConstraints _searchConstraints = new LdapSearchConstraints(
                0,
                0,
                LdapSearchConstraints.DEREF_NEVER,
                1000,
                true,
                1,
                null,
                10);

        private LdapRemoteCertificateValidationCallback _ldapRemoteCertValidator;

        /// <summary>
        /// Initializes a new instance of the <see cref="LdapPasswordChangeProvider"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="options">The _options.</param>
        public LdapPasswordChangeProvider(
            ILogger<LdapPasswordChangeProvider> logger,
            IOptions<LdapPasswordChangeOptions> options)
        {
            _logger = logger;
            _options = options?.Value;

            Init();
        }

        /// <inheritdoc />
        /// <remarks>
        /// Based on:
        ///    * https://www.cs.bham.ac.uk/~smp/resources/ad-passwds/
        ///    * https://support.microsoft.com/en-us/help/269190/how-to-change-a-windows-active-directory-and-lds-user-password-through
        ///    * https://ltb-project.org/documentation/self-service-password/latest/config_ldap#active_directory
        ///    * https://technet.microsoft.com/en-us/library/ff848710.aspx?f=255&amp;MSPPError=-2147217396
        ///
        /// Check the above links for more information.
        /// </remarks>
        public ApiErrorItem PerformPasswordChange(
            string username,
            string currentPassword,
            string newPassword)
        {
            try
            {
                var cleanUsername = CleaningUsername(username);

                var searchFilter = _options.LdapSearchFilter.Replace("{Username}", cleanUsername);

                using (var ldap = BindToLdap())
                {
                    var search = ldap.Search(
                        _options.LdapSearchBase,
                        LdapConnection.SCOPE_SUB,
                        searchFilter,
                        new[] { "distinguishedName" },
                        false,
                        _searchConstraints);

                    // We cannot use search.Count here -- apparently it does not
                    // wait for the results to return before resolving the count
                    // but fortunately hasMore seems to block until final result
                    if (!search.hasMore())
                    {
                        _logger.LogWarning("unable to find username: [{0}]", cleanUsername);

                        return new ApiErrorItem(
                            _options.HideUserNotFound ? ApiErrorCode.InvalidCredentials : ApiErrorCode.UserNotFound,
                            _options.HideUserNotFound ? "invalid credentials" : "username could not be located");
                    }

                    if (search.Count > 1)
                    {
                        _logger.LogWarning("found multiple with same username: [{0}]", cleanUsername);

                        // Hopefully this should not ever happen if AD is preserving SAM Account Name
                        // uniqueness constraint, but just in case, handling this corner case
                        return new ApiErrorItem(ApiErrorCode.UserNotFound, "multiple matching user entries resolved");
                    }

                    var userDN = search.next().DN;

                    if (_options.LdapChangePasswordWithDelAdd)
                    {
                        ChangePasswordDelAdd(currentPassword, newPassword, ldap, userDN);
                    }
                    else
                    {
                        ChangePasswordReplace(newPassword, ldap, userDN);
                    }

                    if (_options.LdapStartTls)
                        ldap.StopTls();

                    ldap.Disconnect();
                }
            }
            catch (LdapException ex)
            {
                var item = ParseLdapException(ex);

                _logger.LogWarning(item.Message, ex);

                return item;
            }
            catch (Exception ex)
            {
                var item = ex is ApiErrorException apiError
                    ? apiError.ToApiErrorItem()
                    : new ApiErrorItem(ApiErrorCode.InvalidCredentials, $"Failed to update password: {ex.Message}");

                _logger.LogWarning(item.Message, ex);

                return item;
            }

            // Everything seems to have worked:
            return null;
        }

        private static void ChangePasswordReplace(string newPassword, ILdapConnection ldap, string userDN)
        {
            // If you don't have the rights to Add and/or Delete the Attribute, you might have the right to change the password-attribute.
            // In this case uncomment the next 2 lines and comment the region 'Change Password by Delete/Add'
            var attribute = new LdapAttribute("userPassword", newPassword);
            var ldapReplace = new LdapModification(LdapModification.REPLACE, attribute);
            ldap.Modify(userDN, new[] { ldapReplace }); // Change with Replace
        }

        private static void ChangePasswordDelAdd(string currentPassword, string newPassword, ILdapConnection ldap, string userDN)
        {
            var oldPassBytes = Encoding.Unicode.GetBytes($@"""{currentPassword}""")
                .Select(x => (sbyte)x).ToArray();
            var newPassBytes = Encoding.Unicode.GetBytes($@"""{newPassword}""")
                .Select(x => (sbyte)x).ToArray();

            var oldAttr = new LdapAttribute("unicodePwd", oldPassBytes);
            var newAttr = new LdapAttribute("unicodePwd", newPassBytes);

            var ldapDel = new LdapModification(LdapModification.DELETE, oldAttr);
            var ldapAdd = new LdapModification(LdapModification.ADD, newAttr);
            ldap.Modify(userDN, new[] { ldapDel, ldapAdd }); // Change with Delete/Add
        }
        
        private static ApiErrorItem ParseLdapException(LdapException ex)
        {
            // If the LDAP server returned an error, it will be formatted
            // similar to this:
            //    "0000052D: AtrErr: DSID-03191083, #1:\n\t0: 0000052D: DSID-03191083, problem 1005 (CONSTRAINT_ATT_TYPE), data 0, Att 9005a (unicodePwd)\n\0"
            //
            // The leading number before the ':' is the Win32 API Error Code in HEX
            if (ex.LdapErrorMessage == null)
            {
                return new ApiErrorItem(ApiErrorCode.Generic, $"Unexpected null exception");
            } 

            var m = Regex.Match(ex.LdapErrorMessage, "([0-9a-fA-F]+):");

            if (!m.Success)
            {
                return new ApiErrorItem(ApiErrorCode.Generic, $"Unexpected error: {ex.LdapErrorMessage}");
            }

            var errCodeString = m.Groups[1].Value;
            var errCode = int.Parse(errCodeString, NumberStyles.HexNumber);
            var err = Win32ErrorCode.ByCode(errCode);

            return err == null
                ? new ApiErrorItem(ApiErrorCode.Generic, $"Unexpected Win32 API error; error code: {errCodeString}")
                : new ApiErrorItem(ApiErrorCode.InvalidCredentials,
                    $"Resolved Win32 API Error: code={err.Code} name={err.CodeName} desc={err.Description}");
        }

        private string CleaningUsername(string username)
        {
            var cleanUsername = username;
            var index = cleanUsername.IndexOf("@", StringComparison.Ordinal);
            if (index >= 0)
                cleanUsername = cleanUsername.Substring(0, index);

            // Must sanitize the username to eliminate the possibility of injection attacks:
            //    * https://docs.microsoft.com/en-us/windows/desktop/adschema/a-samaccountname
            //    * https://docs.microsoft.com/en-us/previous-versions/windows/it-pro/windows-2000-server/bb726984(v=technet.10)
            var invalidChars = "\"/\\[]:;|=,+*?<>\r\n\t".ToCharArray();

            if (cleanUsername.IndexOfAny(invalidChars) >= 0)
            {
                throw new ApiErrorException("username contains one or more invalid characters", ApiErrorCode.InvalidCredentials);
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

            return cleanUsername;
        }

        private void Init()
        {
            // Validate required options
            if (_options.LdapIgnoreTlsErrors || _options.LdapIgnoreTlsValidation)
                _ldapRemoteCertValidator = CustomServerCertValidation;

            if (_options.LdapHostnames?.Length < 1)
            {
                throw new ArgumentException("_options must specify at least one LDAP hostname",
                    nameof(_options.LdapHostnames));
            }

            if (string.IsNullOrEmpty(_options.LdapUsername))
            {
                throw new ArgumentException("_options missing or invalid LDAP bind distinguished name (DN)",
                    nameof(_options.LdapUsername));
            }

            if (string.IsNullOrEmpty(_options.LdapPassword))
            {
                throw new ArgumentException("_options missing or invalid LDAP bind password",
                    nameof(_options.LdapPassword));
            }

            if (string.IsNullOrEmpty(_options.LdapSearchBase))
            {
                throw new ArgumentException("_options must specify LDAP search base",
                    nameof(_options.LdapSearchBase));
            }

            if (string.IsNullOrWhiteSpace(_options.LdapSearchFilter))
            {
                throw new ArgumentException(
                    "No ldapSearchFilter is set. Fill attribute ldapSearchFilter in file appsettings.json",
                    nameof(_options.LdapSearchFilter));
            }

            if (!_options.LdapSearchFilter.Contains("{Username}"))
            {
                throw new ArgumentException(
                    "The ldapSearchFilter should include {{Username}} value in the template string",
                    nameof(_options.LdapSearchFilter));
            }

            // All other configuration is optional, but some may warrant attention
            if (!_options.HideUserNotFound)
                _logger.LogWarning($"option [{nameof(_options.HideUserNotFound)}] is DISABLED; the presence or absence of usernames can be harvested");

            if (!_options.LdapIgnoreTlsErrors)
                _logger.LogWarning($"option [{nameof(_options.LdapIgnoreTlsErrors)}] is ENABLED; invalid certificates will be allowed");
            else if (!_options.LdapIgnoreTlsValidation)
                _logger.LogWarning($"option [{nameof(_options.LdapIgnoreTlsValidation)}] is ENABLED; untrusted certificate roots will be allowed");

            if (_options.LdapPort == LdapConnection.DEFAULT_SSL_PORT && !_options.LdapSecureSocketLayer)
                _logger.LogWarning($"option [{nameof(_options.LdapSecureSocketLayer)}] is DISABLED in combination with standard SSL port [{_options.LdapPort}]");

            if (_options.LdapPort != LdapConnection.DEFAULT_SSL_PORT && !_options.LdapStartTls)
                _logger.LogWarning($"option [{nameof(_options.LdapStartTls)}] is DISABLED in combination with non-standard TLS port [{_options.LdapPort}]");
        }

        private LdapConnection BindToLdap()
        {
            var ldap = new LdapConnection();
            if (_ldapRemoteCertValidator != null)
                ldap.UserDefinedServerCertValidationDelegate += _ldapRemoteCertValidator;

            ldap.SecureSocketLayer = _options.LdapSecureSocketLayer;

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
                    _logger.LogWarning($"failed to connect to host [{h}]", ex);
                }
            }

            if (string.IsNullOrEmpty(bindHostname))
            {
                throw new ApiErrorException("failed to connect to any configured hostname", ApiErrorCode.InvalidCredentials);
            }

            if (_options.LdapStartTls)
                ldap.StartTls();

            ldap.Bind(_options.LdapUsername, _options.LdapPassword);

            return ldap;
        }

        /// <summary>
        /// Custom server certificate validation logic that handles our special
        /// cases based on configuration.  This implements the logic of either
        /// ignoring just untrusted root errors or ignoring all TLS errors.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="certificate">The certificate.</param>
        /// <param name="chain">The chain.</param>
        /// <param name="sslPolicyErrors">The SSL policy errors.</param>
        /// <returns><c>true</c> if the certificate validation was successful.</returns>
        private bool CustomServerCertValidation(
                    object sender,
                    X509Certificate certificate,
                    X509Chain chain,
                    SslPolicyErrors sslPolicyErrors)
        {
            if (_options.LdapIgnoreTlsErrors || sslPolicyErrors == SslPolicyErrors.None)
                return true;

            var errorStatuses = chain.ChainStatus
                .Where(x =>
                {
                    if (x.Status == X509ChainStatusFlags.UntrustedRoot
                        && _options.LdapIgnoreTlsValidation)
                        return false;

                    return x.Status != X509ChainStatusFlags.NoError;
                })
                .ToArray();

            return errorStatuses.Length > 0;
        }
    }
}
