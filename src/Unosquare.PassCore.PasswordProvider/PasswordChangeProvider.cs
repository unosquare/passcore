namespace Unosquare.PassCore.PasswordProvider
{
    using System.DirectoryServices.AccountManagement;
    using System.DirectoryServices;
    using System;
    using Microsoft.Extensions.Options;
    using System.Linq;
    using Common;

    public partial class PasswordChangeProvider : IPasswordChangeProvider
    {
        private readonly PasswordChangeOptions _options;
        private IdentityType _idType = IdentityType.UserPrincipalName;

        public PasswordChangeProvider(IOptions<PasswordChangeOptions> options)
        {
            _options = options.Value;
            SetIdType();
        }

        /// <summary>
        /// 
        /// </summary>
        private void SetIdType()
        {
            if (String.IsNullOrWhiteSpace(_options.IdTypeForUser))
            {
                _idType = IdentityType.UserPrincipalName;
            }
            else
            {
                string tmpIdType = _options.IdTypeForUser.Trim().ToLower();

                switch (tmpIdType)
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
                    case "userprincipalname":
                    case "user principal name":
                    case "upn":
                    case "id":
                    case "cn":
                    case "uid":
                        _idType = IdentityType.UserPrincipalName;
                        break;
                    default:
                        _idType = IdentityType.UserPrincipalName;
                        break;
                }
            }
        }

        public ApiErrorItem PerformPasswordChange(string username, string currentPassword, string newPassword)
        {
            // perform the password change
            try
            {
                using (var principalContext = AcquirePrincipalContext())
                {
                    var userPrincipal = UserPrincipal.FindByIdentity(principalContext, _idType, username);

                    // Check if the user principal exists
                    if (userPrincipal == null)
                    {
                        return new ApiErrorItem { ErrorCode = ApiErrorCode.UserNotFound };
                    }

                    // Check if password change is allowed
                    if (userPrincipal.UserCannotChangePassword)
                    {
                        return new ApiErrorItem { ErrorCode = ApiErrorCode.ChangeNotPermitted };
                    }

                    // Check if password expired or must be changed
                    if (userPrincipal.LastPasswordSet == null)
                    {
                        var der = (DirectoryEntry)userPrincipal.GetUnderlyingObject();
                        var prop = der.Properties["pwdLastSet"];

                        if (prop != null)
                        {
                            prop.Value = -1;
                        }

                        try
                        {
                            der.CommitChanges();
                        }
                        catch (Exception ex)
                        {
                            return new ApiErrorItem { ErrorCode = ApiErrorCode.Generic, Message = ex.Message };
                        }
                    }

                    // Verify user is not a member of an excluded group
                    if (_options.CheckRestrictedAdGroups)
                    {
                        foreach (var userPrincipalAuthGroup in userPrincipal.GetAuthorizationGroups())
                        {
                            if (_options.RestrictedADGroups.Contains(userPrincipalAuthGroup.Name))
                            {
                                return new ApiErrorItem { ErrorCode = ApiErrorCode.ChangeNotPermitted };
                            }
                        }
                    }

                    if (_options.CheckAllowedAdGroups)
                    {
                        foreach (var userPrincipalAuthGroup in userPrincipal.GetAuthorizationGroups())
                        {
                            if (!_options.AllowedADGroups.Contains(userPrincipalAuthGroup.Name))
                            {
                                return new ApiErrorItem { ErrorCode = ApiErrorCode.ChangeNotPermitted };
                            }
                        }
                    }

                    if (ValidateUserCredentials(userPrincipal.UserPrincipalName, currentPassword, principalContext) == false)
                        return new ApiErrorItem { ErrorCode = ApiErrorCode.InvalidCredentials };

                    // Change the password via 2 different methods. Try SetPassword if ChangePassword fails.
                    try
                    {
                        // Try by regular ChangePassword method
                        userPrincipal.ChangePassword(currentPassword, newPassword);
                    }
                    catch
                    {
                        if (_options.UseAutomaticContext) { throw; }

                        // If the previous attempt failed, use the SetPassword method.
                        userPrincipal.SetPassword(newPassword);
                    }

                    userPrincipal.Save();
                }
            }
            catch (Exception ex)
            {
                return new ApiErrorItem { ErrorCode = ApiErrorCode.Generic, Message = ex.Message };
            }

            return null;
        }

        private static bool ValidateUserCredentials(string username, string currentPassword, PrincipalContext principalContext)
        {
            if (principalContext.ValidateCredentials(username, currentPassword))
                return true;

            string tmpAuthority = username?.Split('@')?.Last();
            if (LogonUser(username, tmpAuthority, currentPassword, LogonTypes.Network, LogonProviders.Default, out _))
                return true;

            var errorCode = System.Runtime.InteropServices.Marshal.GetLastWin32Error();

            // Both of these means that the password CAN change and that we got the correct password
            return errorCode == ERROR_PASSWORD_MUST_CHANGE || errorCode == ERROR_PASSWORD_EXPIRED;
        }

        private PrincipalContext AcquirePrincipalContext()
        {
            return _options.UseAutomaticContext
                ? new PrincipalContext(ContextType.Domain)
                : new PrincipalContext(
                    ContextType.Domain,
                    $"{_options.LdapHostname}:{_options.LdapPort}",
                    _options.LdapUsername,
                    _options.LdapPassword);
        }
    }
}