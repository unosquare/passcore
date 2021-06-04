namespace Unosquare.PassCore.PasswordProvider
{
    using Common;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using System;
    using System.DirectoryServices;
    using System.DirectoryServices.AccountManagement;
    using System.DirectoryServices.ActiveDirectory;
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
        }

        /// <inheritdoc />
        public ApiErrorItem? PerformPasswordChange(string username, string currentPassword, string newPassword)
        {
            try
            {
                var fixedUsername = FixUsernameWithDomain(username);
                using var principalContext = AcquirePrincipalContext();
                var userPrincipal = UserPrincipal.FindByIdentity(principalContext, _idType, fixedUsername);

                // Check if the user principal exists
                if (userPrincipal == null)
                {
                    _logger.LogWarning($"The User principal ({fixedUsername}) doesn't exist");

                    return new ApiErrorItem(ApiErrorCode.UserNotFound);
                }

                var minPwdLength = AcquireDomainPasswordLength();

                if (newPassword.Length < minPwdLength)
                {
                    _logger.LogError("Failed due to password complex policies: New password length is shorter than AD minimum password length");

                    return new ApiErrorItem(ApiErrorCode.ComplexPassword);
                }

                // Check if the newPassword is Pwned
                if (PwnedPasswordsSearch.PwnedSearch.IsPwnedPassword(newPassword))
                {
                    _logger.LogError("Failed due to pwned password: New password is publicly known and can be used in dictionary attacks");

                    return new ApiErrorItem(ApiErrorCode.PwnedPassword);
                }

                _logger.LogInformation($"PerformPasswordChange for user {fixedUsername}");

                var item = ValidateGroups(userPrincipal);

                if (item != null)
                    return item;

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

        private bool ValidateUserCredentials(
            string upn,
            string currentPassword,
            PrincipalContext principalContext)
        {
            if (principalContext.ValidateCredentials(upn, currentPassword))
                return true;

            if (NativeMethods.LogonUser(upn, string.Empty, currentPassword, NativeMethods.LogonTypes.Network, NativeMethods.LogonProviders.Default, out _))
                return true;

            var errorCode = System.Runtime.InteropServices.Marshal.GetLastWin32Error();

            _logger.LogDebug($"ValidateUserCredentials GetLastWin32Error {errorCode}");

            // Both of these means that the password CAN change and that we got the correct password
            return errorCode == NativeMethods.ErrorPasswordMustChange || errorCode == NativeMethods.ErrorPasswordExpired;
        }

        private string FixUsernameWithDomain(string username)
        {
            if (_idType != IdentityType.UserPrincipalName) return username;

            // Check for default domain: if none given, ensure EFLD can be used as an override.
            var parts = username.Split(new[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
            var domain = parts.Length > 1 ? parts[1] : _options.DefaultDomain;

            return string.IsNullOrWhiteSpace(domain) || parts.Length > 1 ? username : $"{username}@{domain}";
        }

        private ApiErrorItem? ValidateGroups(UserPrincipal userPrincipal)
        {
            try
            {
                PrincipalSearchResult<Principal> groups;

                try
                {
                    groups = userPrincipal.GetGroups();
                }
                catch (Exception exception)
                {
                    _logger.LogError(new EventId(887), exception, nameof(ValidateGroups));

                    groups = userPrincipal.GetAuthorizationGroups();
                }

                if (_options.RestrictedADGroups != null)
                    if (groups.Any(x => _options.RestrictedADGroups.Contains(x.Name)))
                    {
                        return new ApiErrorItem(ApiErrorCode.ChangeNotPermitted,
                            "The User principal is listed as restricted");
                    }

                return groups?.Any(x => _options.AllowedADGroups?.Contains(x.Name) != false) == true
                    ? null
                    : new ApiErrorItem(ApiErrorCode.ChangeNotPermitted, "The User principal is not listed as allowed");
            }
            catch (Exception exception)
            {
                _logger.LogError(new EventId(888), exception, nameof(ValidateGroups));
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
            _idType = _options.IdTypeForUser?.Trim().ToLower() switch
            {
                "distinguishedname" => IdentityType.DistinguishedName,
                "distinguished name" => IdentityType.DistinguishedName,
                "dn" => IdentityType.DistinguishedName,
                "globally unique identifier" => IdentityType.Guid,
                "globallyuniqueidentifier" => IdentityType.Guid,
                "guid" => IdentityType.Guid,
                "name" => IdentityType.Name,
                "nm" => IdentityType.Name,
                "samaccountname" => IdentityType.SamAccountName,
                "accountname" => IdentityType.SamAccountName,
                "sam account" => IdentityType.SamAccountName,
                "sam account name" => IdentityType.SamAccountName,
                "sam" => IdentityType.SamAccountName,
                "securityidentifier" => IdentityType.Sid,
                "securityid" => IdentityType.Sid,
                "secid" => IdentityType.Sid,
                "security identifier" => IdentityType.Sid,
                "sid" => IdentityType.Sid,
                _ => IdentityType.UserPrincipalName
            };
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

        private int AcquireDomainPasswordLength()
        {
            DirectoryEntry entry;
            if (_options.UseAutomaticContext)
            {
                entry = Domain.GetCurrentDomain().GetDirectoryEntry();
            }
            else
            {
                entry = new DirectoryEntry(
                    $"{_options.LdapHostnames.First()}:{_options.LdapPort}",
                    _options.LdapUsername,
                    _options.LdapPassword
                    );
            }
            return (int)entry.Properties["minPwdLength"].Value;
        }
    }
}
