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

        public PasswordChangeProvider(IOptions<PasswordChangeOptions> options)
        {
            _options = options.Value;
        }

        public ApiErrorItem PerformPasswordChange(string username, string currentPassword, string newPassword)
        {
            // perform the password change
            try
            {
                using (var principalContext = AcquirePrincipalContext())
                {
                    var userPrincipal = UserPrincipal.FindByIdentity(principalContext, username);

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
                        PropertyValueCollection prop = null;
                        DirectoryEntry der = null;
                        der = (DirectoryEntry)userPrincipal.GetUnderlyingObject();

                        prop = der.Properties["pwdLastSet"];

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
                            return new ApiErrorItem { ErrorCode = ApiErrorCode.Generic, Message=ex.Message };
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

                    // Validate user credentials
                    if (principalContext.ValidateCredentials(username, currentPassword) == false)
                    {
                        if (!LogonUser(username, username.Split('@').Last(), currentPassword, LogonTypes.Network, LogonProviders.Default, out _))
                        {
                            var errorCode = System.Runtime.InteropServices.Marshal.GetLastWin32Error();
                            switch (errorCode)
                            {
                                case ERROR_PASSWORD_MUST_CHANGE:
                                case ERROR_PASSWORD_EXPIRED:
                                    // Both of these means that the password CAN change and that we got the correct password
                                    break;
                                default:
                                    return new ApiErrorItem { ErrorCode = ApiErrorCode.InvalidCredentials };
                            }
                        }
                    }

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

        private PrincipalContext AcquirePrincipalContext()
        {
            PrincipalContext principalContext;

            if (_options.UseAutomaticContext)
            {
                principalContext = new PrincipalContext(ContextType.Domain);
            }
            else
            {
                principalContext = new PrincipalContext(
                    ContextType.Domain,
                    $"{_options.LdapHostname}:{_options.LdapPort}",
                    _options.LdapUsername,
                    _options.LdapPassword);
            }

            return principalContext;
        }
    }
}