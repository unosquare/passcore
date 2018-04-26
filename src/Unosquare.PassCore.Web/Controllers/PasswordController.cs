namespace Unosquare.PassCore.Web.Controllers {
    using System.DirectoryServices.AccountManagement;
    using System.Net.Http;
    using System.Net;
    using System.Threading.Tasks;
    using System;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Models;
    using Newtonsoft.Json;

    /// <summary>
    /// Represents a controller class holding all of the server-side functionality of this tool.
    /// </summary>
    [Route ("api/[controller]")]
    public class PasswordController : Controller {
        private readonly AppSettings _options;

        public PasswordController (IOptions<AppSettings> optionsAccessor) {
            _options = optionsAccessor.Value;
        }

        /// <summary>
        /// Returns the ClientSettings object as a JSON string
        /// </summary>
        [HttpGet]
        public IActionResult Get () {
            return Json (_options.ClientSettings);
        }

        /// <summary>
        /// Given a POST request, processes and changes a User's password.
        /// </summary>
        /// <param name="model">The value.</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Post ([FromBody] ChangePasswordModel model) {
            // Validate the request
            if (model == null) {
                return BadRequest (ApiResult.InvalidRequest ());
            }

            var result = new ApiResult ();

            // Validate the model
            if (!ModelState.IsValid) {
                result.AddModelStateErrors (ModelState);
                return BadRequest (result);
            }

            // Validate the Captcha
            try {
                if (await ValidateRecaptcha (model.Recaptcha) == false)
                    result.Errors.Add (new ApiErrorItem { ErrorType = ApiErrorType.GeneralFailure, ErrorCode = ApiErrorCode.InvalidCaptcha });
            } catch (Exception ex) {
                result.Errors.Add (new ApiErrorItem { ErrorType = ApiErrorType.GeneralFailure, ErrorCode = ApiErrorCode.Generic, Message = ex.Message });
            }

            if (result.HasErrors) {
                return BadRequest (result);
            }

            // perform the password change
            try {
                using (var principalContext = AcquirePrincipalContext ()) {
                    var userPrincipal = AcquireUserPricipal (principalContext, model.Username);

                    // Check if the user principal exists
                    if (userPrincipal == null) {
                        result.Errors.Add (new ApiErrorItem { ErrorType = ApiErrorType.GeneralFailure, ErrorCode = ApiErrorCode.UserNotFound, Message = "Invalid Username or Password" });

                        return BadRequest (result);
                    }

                    // Check if password change is allowed
                    if (userPrincipal.UserCannotChangePassword) {
                        throw new Exception (_options.ClientSettings.Alerts.ErrorPasswordChangeNotAllowed);
                    }

                    // Validate user credentials
                    if (principalContext.ValidateCredentials (model.Username, model.CurrentPassword) == false) {
                        // Your new authenticate code snippet
                        IntPtr token = IntPtr.Zero;
                        try {
                            var parts = userPrincipal.UserPrincipalName.Split (new [] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                            string domain = parts.Length > 1 ? parts[1] : null;

                            if (domain == null) {
                                throw new Exception (_options.ClientSettings.Alerts.ErrorInvalidCredentials);
                            }

                            if (!PasswordChangeFallBack.LogonUser (model.Username, domain, model.CurrentPassword, PasswordChangeFallBack.LogonTypes.Network, PasswordChangeFallBack.LogonProviders.Default, out token)) {
                                int errorCode = System.Runtime.InteropServices.Marshal.GetLastWin32Error ();
                                switch (errorCode) {
                                    case PasswordChangeFallBack.ERROR_PASSWORD_MUST_CHANGE:
                                    case PasswordChangeFallBack.ERROR_PASSWORD_EXPIRED:
                                        // Both of these means that the password CAN change and that we got the correct password
                                        break;
                                    default:
                                        throw new Exception (_options.ClientSettings.Alerts.ErrorInvalidCredentials);
                                }
                            }
                        } finally {
                            PasswordChangeFallBack.CloseHandle (token);
                        }
                    }

                    // Verify user is not a member of an excluded group
                    if (_options.ClientSettings.CheckRestrictedAdGroups) {
                        foreach (Principal userPrincipalAuthGroup in userPrincipal.GetAuthorizationGroups ()) {
                            if (_options.ClientSettings.RestrictedADGroups.Contains (userPrincipalAuthGroup.Name)) {
                                throw new Exception (_options.ClientSettings.Alerts.ErrorPasswordChangeNotAllowed);
                            }
                        }
                    }

                    // Change the password via 2 different methods. Try SetPassword if ChangePassword fails.
                    try {
                        // Try by regular ChangePassword method
                        userPrincipal.ChangePassword (model.CurrentPassword, model.NewPassword);
                    } catch (Exception ex2) {
                        // If the previous attempt failed, use the SetPassword method.
                        if (_options.PasswordChangeOptions.UseAutomaticContext == false)
                            userPrincipal.SetPassword (model.NewPassword);
                        else
                            throw ex2;
                    }

                    userPrincipal.Save ();
                }
            } catch (Exception ex) {
                result.Errors.Add (new ApiErrorItem { ErrorType = ApiErrorType.GeneralFailure, ErrorCode = ApiErrorCode.Generic, Message = ex.Message });

                return BadRequest (result);
            }

            if (result.HasErrors)
                Response.StatusCode = (int) HttpStatusCode.BadRequest;

            return Json (result);
        }

        private static UserPrincipal AcquireUserPricipal (PrincipalContext context, string username) {
            return UserPrincipal.FindByIdentity (context, username);
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

        private async Task<bool> ValidateRecaptcha (string recaptchaResponse) {
            // skip validation if we don't enable recaptcha
            if (string.IsNullOrWhiteSpace (_options.RecaptchaPrivateKey)) return true;

            // immediately return false because we don't 
            if (string.IsNullOrEmpty (recaptchaResponse)) return false;

            var requestUrl = $"https://www.google.com/recaptcha/api/siteverify?secret={_options.RecaptchaPrivateKey}&response={recaptchaResponse}";

            using (var client = new HttpClient ()) {
                var response = await client.GetStringAsync (requestUrl);
                dynamic validationResponse = JsonConvert.DeserializeObject (response);
                return validationResponse.success;
            }
        }

        /// <summary>
        /// This code is taken from the answer https://stackoverflow.com/a/1766203 from https://stackoverflow.com/questions/1394025/active-directory-ldap-check-account-locked-out-password-expired
        /// </summary>
        internal static class PasswordChangeFallBack {
            // See http://support.microsoft.com/kb/155012
            internal const int ERROR_PASSWORD_MUST_CHANGE = 1907;
            internal const int ERROR_LOGON_FAILURE = 1326;
            internal const int ERROR_ACCOUNT_RESTRICTION = 1327;
            internal const int ERROR_ACCOUNT_DISABLED = 1331;
            internal const int ERROR_INVALID_LOGON_HOURS = 1328;
            internal const int ERROR_NO_LOGON_SERVERS = 1311;
            internal const int ERROR_INVALID_WORKSTATION = 1329;
            internal const int ERROR_ACCOUNT_LOCKED_OUT = 1909; // It gives this error if the account is locked, REGARDLESS OF WHETHER VALID CREDENTIALS WERE PROVIDED!!!
            internal const int ERROR_ACCOUNT_EXPIRED = 1793;
            internal const int ERROR_PASSWORD_EXPIRED = 1330;

            // here are enums
            internal enum LogonTypes : uint {
                Interactive = 2,
                Network = 3,
                Batch = 4,
                Service = 5,
                Unlock = 7,
                NetworkCleartext = 8,
                NewCredentials = 9
            }

            internal enum LogonProviders : uint {
                Default = 0, // default for platform (use this!)
                WinNT35, // sends smoke signals to authority
                WinNT40, // uses NTLM
                WinNT50 // negotiates Kerb or NTLM
            }

            [System.Runtime.InteropServices.DllImport ("advapi32.dll", SetLastError = true)]
            internal static extern bool LogonUser (
                string principal,
                string authority,
                string password,
                LogonTypes logonType,
                LogonProviders logonProvider,
                out IntPtr token);

            [System.Runtime.InteropServices.DllImport ("kernel32.dll", SetLastError = true)]
            internal static extern bool CloseHandle (IntPtr handle);
        }
    }
}