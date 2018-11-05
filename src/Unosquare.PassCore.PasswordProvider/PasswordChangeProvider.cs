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
    /// <seealso cref="T:Unosquare.PassCore.Common.IPasswordChangeProvider" />
    public partial class PasswordChangeProvider : IPasswordChangeProvider
    {
        private readonly ILogger _logger;
        private IdentityType _idType = IdentityType.UserPrincipalName;

        public PasswordChangeProvider(
            ILogger<PasswordChangeProvider> logger,
            IOptions<PasswordChangeOptions> options)
        {
            _logger = logger;
            Settings = options.Value;
            SetIdType();
        }

        /// <inheritdoc />
        public IAppSettings Settings { get; }

        public PasswordChangeOptions PasswordChangeOptions => Settings as PasswordChangeOptions;

        public ApiErrorItem PerformPasswordChange(string username, string currentPassword, string newPassword)
        {
            _logger.LogInformation($"PerformPasswordChange for user {username}");

            try
            {
                using (var principalContext = AcquirePrincipalContext())
                {
                    var userPrincipal = UserPrincipal.FindByIdentity(principalContext, _idType, username);

                    // Check if the user principal exists
                    if (userPrincipal == null)
                    {
                        _logger.LogWarning($"The User principal ({username}) doesn't exist");

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
                    if (PasswordChangeOptions.UpdateLastPassword && userPrincipal.LastPasswordSet == null)
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
            catch (Exception ex)
            {
                var item = ex is ApiErrorException apiError
                    ? apiError.ToApiErrorItem()
                    : new ApiErrorItem(ApiErrorCode.Generic, $"Failed to update password: {ex.Message}");

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
            return errorCode == ERROR_PASSWORD_MUST_CHANGE || errorCode == ERROR_PASSWORD_EXPIRED;
        }

        private void ValidateGroups(UserPrincipal userPrincipal)
        {
            if (PasswordChangeOptions.RestrictedADGroups?.Any() == true)
            {
                foreach (var userPrincipalAuthGroup in userPrincipal.GetAuthorizationGroups())
                {
                    if (PasswordChangeOptions.RestrictedADGroups.Contains(userPrincipalAuthGroup.Name))
                    {
                        throw new ApiErrorException("The User principal is listed as restricted",
                            ApiErrorCode.ChangeNotPermitted);
                    }
                }
            }

            if (PasswordChangeOptions.AllowedADGroups?.Any() != true) return;

            foreach (var userPrincipalAuthGroup in userPrincipal.GetAuthorizationGroups())
            {
                if (!PasswordChangeOptions.AllowedADGroups.Contains(userPrincipalAuthGroup.Name))
                {
                    throw new ApiErrorException("The User principal is not listed as allowed",
                        ApiErrorCode.ChangeNotPermitted);
                }
            }
        }

        private void SetLastPassword(UserPrincipal userPrincipal)
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
            UserPrincipal userPrincipal)
        {
            try
            {
                // Try by regular ChangePassword method
                userPrincipal.ChangePassword(currentPassword, newPassword);
            }
            catch
            {
                if (PasswordChangeOptions.UseAutomaticContext)
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
            switch (PasswordChangeOptions.IdTypeForUser?.Trim().ToLower())
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
            if (PasswordChangeOptions.UseAutomaticContext)
            {
                _logger.LogWarning("Using AutomaticContext");
                return new PrincipalContext(ContextType.Domain);
            }

            var domain = $"{Settings.LdapHostnames.First()}:{Settings.LdapPort}";
            _logger.LogWarning($"Not using AutomaticContext  {domain}");

            return new PrincipalContext(
                ContextType.Domain,
                domain,
                Settings.LdapUsername,
                Settings.LdapPassword);
        }
    }
}