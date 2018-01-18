namespace Unosquare.PassCore.Web.Controllers
{
    using Microsoft.Extensions.Options;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Models;
    using System.DirectoryServices.AccountManagement;
    using System.Collections.Generic;
#if SWAN
    using System.Collections;
    using System.Linq;
    using Unosquare.Swan;
    using Unosquare.Swan.Networking.Ldap;
#endif

    /// <summary>
    /// Represents a controller class holding all of the server-side functionality of this tool.
    /// </summary>
    [Route("api/[controller]")]
    public class PasswordController : Controller
    {
        private readonly AppSettings _options;

        public PasswordController(IOptions<AppSettings> optionsAccessor)
        {
            _options = optionsAccessor.Value;
        }

        /// <summary>
        /// Returns the ClientSettings object as a JSON string
        /// </summary>
        [HttpGet]
        public IActionResult Get()
        {
            return Json(_options.ClientSettings);
        }

        /// <summary>
        /// Given a POST request, processes and changes a User's password.
        /// </summary>
        /// <param name="model">The value.</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]ChangePasswordModel model)
        {
            // Validate the request
            if (model == null)
            {
                return BadRequest(ApiResult.InvalidRequest());
            }

            var result = new ApiResult();

            // Validate the model
            if (ModelState.IsValid == false)
            {
                result.AddModelStateErrors(ModelState);

                return BadRequest(result);
            }

            // Validate the Captcha
            try
            {
                if (await ValidateRecaptcha(model.Recaptcha) == false)
                    result.Errors.Add(new ApiErrorItem { ErrorType = ApiErrorType.GeneralFailure, ErrorCode = ApiErrorCode.InvalidCaptcha });
            }
            catch (Exception ex)
            {
                result.Errors.Add(new ApiErrorItem { ErrorType = ApiErrorType.GeneralFailure, ErrorCode = ApiErrorCode.Generic, Message = ex.Message });
            }

            if (result.HasErrors)
            {
                return BadRequest(result);
            }

            // perform the password change
            try
            {
#if SWAN
                var distinguishedName = await GetDN(model.Username);

                if (string.IsNullOrEmpty(distinguishedName))
                {
                    result.Errors.Add(new ApiErrorItem() { ErrorType = ApiErrorType.GeneralFailure, ErrorCode = ApiErrorCode.InvalidCredentials, Message = "Invalid Username or Password" });
                    
                    return BadRequest(result);
                }

                var cn = new LdapConnection();

                await cn.Connect(_options.PasswordChangeOptions.LdapHostname, _options.PasswordChangeOptions.LdapPort);
                await cn.Bind(_options.PasswordChangeOptions.LdapUsername, _options.PasswordChangeOptions.LdapPassword);
                var modList = new ArrayList();
                var attribute = new LdapAttribute("userPassword", model.NewPassword);
                modList.Add(new LdapModification(LdapModificationOp.Replace, attribute));
                var mods = (LdapModification[])modList.ToArray(typeof(LdapModification));
                await cn.Modify(distinguishedName, mods);
                cn.Disconnect();
#else
                using (var principalContext = AcquirePrincipalContext())
                {
                    var userPrincipal = AcquireUserPricipal(principalContext, model.Username);

                    // Check if the user principal exists
                    if (userPrincipal == null)
                    {
                        result.Errors.Add(new ApiErrorItem { ErrorType = ApiErrorType.GeneralFailure, ErrorCode = ApiErrorCode.UserNotFound, Message = "Invalid Username or Password" });
                        
                        return BadRequest(result);
                    }
                    else
                    {
                        Console.WriteLine("Attempting to change password for: " + userPrincipal.Name);
                    }
                    
                    // Check if password change is allowed
                    if (userPrincipal.UserCannotChangePassword)
                    {
                        Console.WriteLine("ERROR: " + userPrincipal.Name + " is not allowed to change their password");
                        throw new Exception(_options.ClientSettings.Alerts.ErrorPasswordChangeNotAllowed);
                    }

                    // Validate user credentials
                    if (principalContext.ValidateCredentials(model.Username, model.CurrentPassword) == false)
                    {
                        Console.WriteLine("ERROR: " + userPrincipal.Name + " entered invalid current credentials.");
                        throw new Exception(_options.ClientSettings.Alerts.ErrorInvalidCredentials);
                    }

                    // Verify user is not a member of an excluded group
                    if (_options.ClientSettings.CheckRestrictedAdGroups)
                    {
                        Console.WriteLine("Checking for restricted group membership.");

                        foreach (Principal userPrincipalAuthGroup in userPrincipal.GetAuthorizationGroups())
                        {
                            Console.WriteLine(userPrincipal.Name + " is a member of " + userPrincipalAuthGroup.Name + ".");

                            if (_options.ClientSettings.RestrictedADGroups.Contains(userPrincipalAuthGroup.Name))
                            {
                                Console.WriteLine("ERROR: " + userPrincipal.Name + " is a member of " + userPrincipalAuthGroup.Name + "!");
                                throw new Exception(_options.ClientSettings.Alerts.ErrorPasswordChangeNotAllowed);
                            }
                        }
                    }

                    // Change the password via 2 different methods. Try SetPassword if ChangePassword fails.
                    try
                    {
                        // Try by regular ChangePassword method
                        userPrincipal.ChangePassword(model.CurrentPassword, model.NewPassword);
                        Console.WriteLine("SUCCESS: " + userPrincipal.Name + " has changed their password.");
                    }
                    catch (Exception ex2)
                    {
                        // If the previous attempt failed, use the SetPassword method.
                        if (_options.PasswordChangeOptions.UseAutomaticContext == false)
                        {
                            userPrincipal.SetPassword(model.NewPassword);
                            Console.WriteLine("SUCCESS: " + userPrincipal.Name + " has changed their password.");
                        }
                        else
                        {
                            throw ex2;
                        }                            
                    }

                    userPrincipal.Save();
                    userPrincipal.Dispose();
                }
#endif
            }
            catch (Exception ex)
            {
                result.Errors.Add(new ApiErrorItem { ErrorType = ApiErrorType.GeneralFailure, ErrorCode = ApiErrorCode.Generic, Message = ex.Message });
                
                return BadRequest(result);
            }

            if (result.HasErrors)
                Response.StatusCode = (int)HttpStatusCode.BadRequest;

            return Json(result);
        }

        private static UserPrincipal AcquireUserPricipal(PrincipalContext context, string username)
        {
            return UserPrincipal.FindByIdentity(context, username);
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

#if SWAN
        private string GetBase()
        {
            var _base = string.Empty;
            var hostName = Settings.PasswordChangeOptions.LdapHostname.Split('.');

            foreach (var part in hostName)
            {
                if (part == hostName.Last())
                    _base += $"dc={part}";
                else
                    _base += $"dc={part},";
            }

            return _base;
        }

        private async Task<string> GetDN(string username)
        {
            try
            {
                var cn = new LdapConnection();
                var distinguishedName = string.Empty;
                await cn.Connect(Settings.PasswordChangeOptions.LdapHostname, Settings.PasswordChangeOptions.LdapPort);
                await cn.Bind(Settings.PasswordChangeOptions.LdapUsername, Settings.PasswordChangeOptions.LdapPassword);
                var lsc = await cn.Search(GetBase(), LdapConnection.ScopeSub, $"(mail={username})");

                while (lsc.HasMore())
                {
                    var entry = lsc.Next();
                    var ldapAttributes = entry.GetAttributeSet();
                    distinguishedName = ldapAttributes.GetAttribute("distinguishedName")?.StringValue ?? string.Empty;
                }

                cn.Disconnect();
                return distinguishedName;
            }
            catch (Exception ex)
            {
                ex.Error(nameof(GetDN), "Error LDAP");
                return string.Empty;
            }
        }
#endif
        private async Task<bool> ValidateRecaptcha(string recaptchaResponse)
        {
            // skip validation if we don't enable recaptcha
            if (string.IsNullOrWhiteSpace(_options.RecaptchaPrivateKey)) return true;

            // immediately return false because we don't 
            if (string.IsNullOrEmpty(recaptchaResponse)) return false;

            var requestUrl = $"https://www.google.com/recaptcha/api/siteverify?secret={_options.RecaptchaPrivateKey}&response={recaptchaResponse}";

            using (var client = new HttpClient())
            {
                var response = await client.GetStringAsync(requestUrl);
                dynamic validationResponse = JsonConvert.DeserializeObject(response);
                return validationResponse.success;
            }
        }
    }
}
