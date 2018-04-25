namespace Unosquare.PassCore.Web.Helpers {
    using System.DirectoryServices.AccountManagement;
    using System;
    using Microsoft.Extensions.Options;
    using Models;

    internal class PasswordChangeProvider : IPasswordChangeProvider {
        private readonly AppSettings _options;

        public PasswordChangeProvider (IOptions<AppSettings> options) {
            _options = options.Value;
        }

        public ApiErrorItem PerformPasswordChange (ChangePasswordModel model) {
            // perform the password change
            try {
                using (var principalContext = AcquirePrincipalContext ()) {
                    var userPrincipal = UserPrincipal.FindByIdentity (principalContext, model.Username);

                    // Check if the user principal exists
                    if (userPrincipal == null) {
                        return new ApiErrorItem { ErrorType = ApiErrorType.GeneralFailure, ErrorCode = ApiErrorCode.UserNotFound, Message = "Invalid Username or Password" };
                    }

                    // Check if password change is allowed
                    if (userPrincipal.UserCannotChangePassword) {
                        throw new ArgumentOutOfRangeException (_options.ClientSettings.Alerts.ErrorPasswordChangeNotAllowed);
                    }

                    // Validate user credentials
                    if (principalContext.ValidateCredentials (model.Username, model.CurrentPassword) == false) {
                        // Your new authenticate code snippet
                        var token = IntPtr.Zero;
                        try {
                            var parts = userPrincipal.UserPrincipalName.Split (new [] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                            // Check for default domain, if none given
                            var domain = _options.ClientSettings.DefaultDomain ?? (parts.Length > 1 ? parts[1] : null);

                            if (domain == null) {
                                throw new ArgumentOutOfRangeException (_options.ClientSettings.Alerts.ErrorInvalidCredentials);
                            }

                            if (!PasswordChangeFallBack.LogonUser (model.Username, domain, model.CurrentPassword, PasswordChangeFallBack.LogonTypes.Network, PasswordChangeFallBack.LogonProviders.Default, out token)) {
                                var errorCode = System.Runtime.InteropServices.Marshal.GetLastWin32Error ();
                                switch (errorCode) {
                                    case PasswordChangeFallBack.ERROR_PASSWORD_MUST_CHANGE:
                                    case PasswordChangeFallBack.ERROR_PASSWORD_EXPIRED:
                                        // Both of these means that the password CAN change and that we got the correct password
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException (_options.ClientSettings.Alerts.ErrorInvalidCredentials);
                                }
                            }
                        } finally {
                            PasswordChangeFallBack.CloseHandle (token);
                        }
                    }

                    // Verify user is not a member of an excluded group
                    if (_options.ClientSettings.CheckRestrictedAdGroups) {
                        foreach (var userPrincipalAuthGroup in userPrincipal.GetAuthorizationGroups ()) {
                            if (_options.ClientSettings.RestrictedADGroups.Contains (userPrincipalAuthGroup.Name)) {
                                throw new Exception (_options.ClientSettings.Alerts.ErrorPasswordChangeNotAllowed);
                            }
                        }
                    }

                    // Change the password via 2 different methods. Try SetPassword if ChangePassword fails.
                    try {
                        // Try by regular ChangePassword method
                        userPrincipal.ChangePassword (model.CurrentPassword, model.NewPassword);
                    } catch {
                        if (_options.PasswordChangeOptions.UseAutomaticContext) { throw; }
                        // If the previous attempt failed, use the SetPassword method.
                        userPrincipal.SetPassword (model.NewPassword);
                    }

                    userPrincipal.Save ();
                }
            } catch (Exception ex) {
                return new ApiErrorItem { ErrorType = ApiErrorType.GeneralFailure, ErrorCode = ApiErrorCode.Generic, Message = ex.Message };
            }

            return null;
        }

        private PrincipalContext AcquirePrincipalContext () {
            PrincipalContext principalContext;

            if (_options.PasswordChangeOptions.UseAutomaticContext) {
                principalContext = new PrincipalContext (ContextType.Domain);
            } else {
                principalContext = new PrincipalContext (
                    ContextType.Domain,
                    $"{_options.PasswordChangeOptions.LdapHostname}:{_options.PasswordChangeOptions.LdapPort}",
                    _options.PasswordChangeOptions.LdapUsername,
                    _options.PasswordChangeOptions.LdapPassword);
            }

            return principalContext;
        }
    }
}