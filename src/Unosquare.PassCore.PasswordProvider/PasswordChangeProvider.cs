namespace Unosquare.PassCore.PasswordProvider
{
    using Common;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using System;
    using System.DirectoryServices;
    using System.DirectoryServices.AccountManagement;
    using System.Linq;

    /// <inheritdoc />
    /// <summary>
    /// Default Change Password Provider using 'System.DirectoryServices' from Microsoft.
    /// </summary>
    /// <seealso cref="IPasswordChangeProvider" />
    public partial class PasswordChangeProvider : IPasswordChangeProvider
    {
        private readonly PasswordChangeOptions _options;
        private readonly ILogger _logger;
        private IdentityType _idType = IdentityType.UserPrincipalName;
        private readonly DomainPasswordInformation? _domainPasswordInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="PasswordChangeProvider"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="options">The options.</param>
        public PasswordChangeProvider(
            ILogger<PasswordChangeProvider> logger,
            IOptions<PasswordChangeOptions> options)
        {
            _logger = logger;
            _options = options.Value;
            SetIdType();
            _domainPasswordInfo = GetDomainPasswordInformation();
        }

        /// <inheritdoc />
        public ApiErrorItem PerformPasswordChange(string username, string currentPassword, string newPassword)
        {
            var fixedUsername = FixUsernameWithDomain(username);
            _logger.LogInformation($"PerformPasswordChange for user {fixedUsername}");

            if (_domainPasswordInfo != null && newPassword.Length < _domainPasswordInfo.Value.MinPasswordLength)
            {
                _logger.LogError("Failed due to password complex policies: New password length is shorter than AD minimum password length");

                return new ApiErrorItem(ApiErrorCode.ComplexPassword);
            }

            try
            {

                using (var principalContext = AcquirePrincipalContext())
                {
                    var userPrincipal = UserPrincipal.FindByIdentity(principalContext, _idType, fixedUsername);

                    // Check if the user principal exists
                    if (userPrincipal == null)
                    {
                        _logger.LogWarning($"The User principal ({fixedUsername}) doesn't exist");

                        return new ApiErrorItem(ApiErrorCode.UserNotFound);
                    }

                    ValidateGroups(userPrincipal);

                    // Check if password change is allowed
                    if (userPrincipal.UserCannotChangePassword)
                    {
                        _logger.LogWarning("The User principal cannot change the password");

                        return new ApiErrorItem(ApiErrorCode.ChangeNotPermitted);
                    }

                    // Check if password expired or must be changed
                    if (_options.UpdateLastPassword && userPrincipal.LastPasswordSet == null)
                    {
                        SetLastPassword(userPrincipal);
                    }

                    // Use always UPN for password check.
                    if (!ValidateUserCredentials(userPrincipal.UserPrincipalName, currentPassword, principalContext))
                    {
                        _logger.LogWarning("The User principal password is not valid");

                        return new ApiErrorItem(ApiErrorCode.InvalidCredentials);
                    }

                    // Change the password via 2 different methods. Try SetPassword if ChangePassword fails.
                    ChangePassword(currentPassword, newPassword, userPrincipal);

                    userPrincipal.Save();
                    _logger.LogDebug("The User principal password updated with setPassword");
                }
            }
            catch (PasswordException passwordEx)
            {
                var item = new ApiErrorItem(ApiErrorCode.ComplexPassword, passwordEx.Message);
                
                _logger.LogWarning(item.Message, passwordEx);

                return item;
            }
            catch (Exception ex)
            {
                var item = ex is ApiErrorException apiError
                    ? apiError.ToApiErrorItem()
                    : new ApiErrorItem(ApiErrorCode.Generic, ex.InnerException?.Message ?? ex.Message);

                _logger.LogWarning(item.Message, ex);

                return item;
            }

            return null;
        }

        private static bool ValidateUserCredentials(
            string upn,
            string currentPassword,
            PrincipalContext principalContext)
        {
            if (principalContext.ValidateCredentials(upn, currentPassword))
                return true;

            var tmpAuthority = upn?.Split('@').Last();

            if (LogonUser(upn, tmpAuthority, currentPassword, LogonTypes.Network, LogonProviders.Default, out _))
                return true;

            var errorCode = System.Runtime.InteropServices.Marshal.GetLastWin32Error();

            // Both of these means that the password CAN change and that we got the correct password
            return errorCode == ErrorPasswordMustChange || errorCode == ErrorPasswordExpired;
        }

        private string FixUsernameWithDomain(string username)
        {
            if (_idType != IdentityType.UserPrincipalName) return username;

            // Check for default domain: if none given, ensure EFLD can be used as an override.
            var parts = username.Split(new[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
            var domain = parts.Length > 1 ? parts[1] : _options.DefaultDomain;

            return string.IsNullOrWhiteSpace(domain) || parts.Length > 1 ? username : $"{username}@{domain}";
        }

        private void ValidateGroups(UserPrincipal userPrincipal)
        {
            try
            {
                if (!userPrincipal.GetGroups().ToList().Any()) return;

                if (_options.RestrictedADGroups?.Any() == true)
                {
                    foreach (var userPrincipalAuthGroup in userPrincipal.GetAuthorizationGroups())
                    {
                        if (_options.RestrictedADGroups.Contains(userPrincipalAuthGroup.Name))
                        {
                            throw new ApiErrorException("The User principal is listed as restricted",
                                ApiErrorCode.ChangeNotPermitted);
                        }
                    }
                }

                if (_options.AllowedADGroups?.Any() != true) return;

                foreach (var userPrincipalAuthGroup in userPrincipal.GetAuthorizationGroups())
                {
                    if (_options.AllowedADGroups.Contains(userPrincipalAuthGroup.Name))
                    {
                        return;
                    }
                }

                // If after iterate the user groups the user cannot change password.
                throw new ApiErrorException("The User principal is not listed as allowed",
                    ApiErrorCode.ChangeNotPermitted);
            }
            catch (ApiErrorException)
            {
                throw;
            }
            catch (Exception exception)
            {
                _logger.LogError(new EventId(888), exception, nameof(ValidateGroups));
            }
        }

        private DomainPasswordInformation? GetDomainPasswordInformation()
        {
            using (var server = new SamServer())
            {
                foreach (var domain in server.EnumerateDomains())
                {
                    if (domain == "Builtin") continue;
                    if (!string.IsNullOrEmpty(_options.DefaultDomain) && !_options.DefaultDomain.Contains(domain)) continue;

                    var sid = server.GetDomainSid(domain);
                    return server.GetDomainPasswordInformation(sid);
                }
            }

            return null;
        }

        private void SetLastPassword(Principal userPrincipal)
        {
            var directoryEntry = (DirectoryEntry)userPrincipal.GetUnderlyingObject();
            var prop = directoryEntry.Properties["pwdLastSet"];

            if (prop == null)
            {
                _logger.LogWarning("The User principal password have no last password, but the property is missing");
                return;
            }

            try
            {
                prop.Value = -1;
                directoryEntry.CommitChanges();
                _logger.LogWarning("The User principal last password was updated");
            }
            catch (Exception ex)
            {
                throw new ApiErrorException($"Failed to update password: {ex.Message}",
                    ApiErrorCode.ChangeNotPermitted);
            }
        }

        private void ChangePassword(
            string currentPassword,
            string newPassword,
            AuthenticablePrincipal userPrincipal)
        {
            try
            {
                // Try by regular ChangePassword method
                userPrincipal.ChangePassword(currentPassword, newPassword);
            }
            catch
            {
                if (_options.UseAutomaticContext)
                {
                    _logger.LogWarning("The User principal password cannot be changed and setPassword won't be called");

                    throw;
                }

                // If the previous attempt failed, use the SetPassword method.
                userPrincipal.SetPassword(newPassword);

                _logger.LogDebug("The User principal password updated with setPassword");
            }
        }

        /// <summary>
        /// Use the values from appsettings.IdTypeForUser as fault-tolerant as possible.
        /// </summary>
        private void SetIdType()
        {
            switch (_options.IdTypeForUser?.Trim().ToLower())
            {
                case "distinguishedname":
                case "distinguished name":
                case "dn":
                    _idType = IdentityType.DistinguishedName;
                    break;
                case "globally unique identifier":
                case "globallyuniqueidentifier":
                case "guid":
                    _idType = IdentityType.Guid;
                    break;
                case "name":
                case "nm":
                    _idType = IdentityType.Name;
                    break;
                case "samaccountname":
                case "accountname":
                case "sam account":
                case "sam account name":
                case "sam":
                    _idType = IdentityType.SamAccountName;
                    break;
                case "securityidentifier":
                case "securityid":
                case "secid":
                case "security identifier":
                case "sid":
                    _idType = IdentityType.Sid;
                    break;
                default:
                    _idType = IdentityType.UserPrincipalName;
                    break;
            }
        }

        private PrincipalContext AcquirePrincipalContext()
        {
            if (_options.UseAutomaticContext)
            {
                _logger.LogWarning("Using AutomaticContext");
                return new PrincipalContext(ContextType.Domain);
            }

            var domain = $"{_options.LdapHostnames.First()}:{_options.LdapPort}";
            _logger.LogWarning($"Not using AutomaticContext  {domain}");

            return new PrincipalContext(
                ContextType.Domain,
                domain,
                _options.LdapUsername,
                _options.LdapPassword);
        }
    }
}