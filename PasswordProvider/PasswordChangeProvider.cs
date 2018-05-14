namespace PasswordProvider
{
    using System.DirectoryServices.AccountManagement;
    using System;
    using Microsoft.Extensions.Options;

    public partial class PasswordChangeProvider : IPasswordChangeProvider
    {
        private readonly  _options;

        public PasswordChangeProvider(IOptions<> options)
        {
            _options = options.Value;
        }

        public ApiErrorItem PerformPasswordChange(ChangePasswordModel model)
        {
            // perform the password change
            try
            {
                // Check for default domain: if none given, ensure EFLD can be used as an override.
                var parts = model.Username.Split(new[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                var domain = parts.Length > 1 ? parts[1] : _options.ClientSettings.DefaultDomain;

                // Domain-determinance
                if (string.IsNullOrEmpty(domain))
                {
                    return new ApiErrorItem { ErrorCode = ApiErrorCode.InvalidDomain, Message = _options.ClientSettings.Alerts.ErrorInvalidDomain };
                }

                var username = parts.Length > 1 ? model.Username : $"{model.Username}@{domain}";

                using (var principalContext = AcquirePrincipalContext())
                {
                    var userPrincipal = UserPrincipal.FindByIdentity(principalContext, username);

                    // Check if the user principal exists
                    if (userPrincipal == null)
                    {
                        return new ApiErrorItem { ErrorCode = ApiErrorCode.UserNotFound, Message = _options.ClientSettings.Alerts.ErrorInvalidUser };
                    }

                    // Check if password change is allowed
                    if (userPrincipal.UserCannotChangePassword)
                    {
                        return new ApiErrorItem { ErrorCode = ApiErrorCode.ChangeNotPermitted, Message = _options.ClientSettings.Alerts.ErrorPasswordChangeNotAllowed };
                    }

                    // Verify user is not a member of an excluded group
                    if (_options.ClientSettings.CheckRestrictedAdGroups)
                    {
                        foreach (var userPrincipalAuthGroup in userPrincipal.GetAuthorizationGroups())
                        {
                            if (_options.ClientSettings.RestrictedADGroups.Contains(userPrincipalAuthGroup.Name))
                            {
                                return new ApiErrorItem { ErrorCode = ApiErrorCode.ChangeNotPermitted, Message = _options.ClientSettings.Alerts.ErrorPasswordChangeNotAllowed };
                            }
                        }
                    }

                    // Validate user credentials
                    if (principalContext.ValidateCredentials(model.Username, model.CurrentPassword) == false)
                    {
                        if (!LogonUser(username, domain, model.CurrentPassword, LogonTypes.Network, LogonProviders.Default, out _))
                        {
                            var errorCode = System.Runtime.InteropServices.Marshal.GetLastWin32Error();
                            switch (errorCode)
                            {
                                case ERROR_PASSWORD_MUST_CHANGE:
                                case ERROR_PASSWORD_EXPIRED:
                                    // Both of these means that the password CAN change and that we got the correct password
                                    break;
                                default:
                                    return new ApiErrorItem { ErrorCode = ApiErrorCode.InvalidCredentials, Message = _options.ClientSettings.Alerts.ErrorInvalidCredentials };
                            }
                        }
                    }

                    // Change the password via 2 different methods. Try SetPassword if ChangePassword fails.
                    try
                    {
                        // Try by regular ChangePassword method
                        userPrincipal.ChangePassword(model.CurrentPassword, model.NewPassword);
                    }
                    catch
                    {
                        if (_options.PasswordChangeOptions.UseAutomaticContext) { throw; }

                        // If the previous attempt failed, use the SetPassword method.
                        userPrincipal.SetPassword(model.NewPassword);
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

            if (_options.PasswordChangeOptions.UseAutomaticContext)
            {
                principalContext = new PrincipalContext(ContextType.Domain);
            }
            else
            {
                principalContext = new PrincipalContext(
                    ContextType.Domain,
                    $"{_options.PasswordChangeOptions.LdapHostname}:{_options.PasswordChangeOptions.LdapPort}",
                    _options.PasswordChangeOptions.LdapUsername,
                    _options.PasswordChangeOptions.LdapPassword);
            }

            return principalContext;
        }
    }
}