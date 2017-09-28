namespace Unosquare.PassCore.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json;
    using System;
    using System.Collections;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Unosquare.PassCore.Web.Models;
    using Unosquare.Swan;
    using Unosquare.Swan.Networking.Ldap;


    /// <summary>
    /// Represents a controller class holding all of the server-side functionality of this tool.
    /// </summary>
    [Route("api/[controller]")]
    public class PasswordController : ControllerBase
    {
        public PasswordController(IConfigurationRoot configuration)
            : base(configuration)
        {
            // placeholder
        }

        /// <summary>
        /// Returns the ClientSettings object as a JSON string
        /// </summary>
        [HttpGet]
        public IActionResult Get()
        {
            return Json(Settings.ClientSettings);
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
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(ApiResult.InvalidRequest());
            }

            var result = new ApiResult();

            // Validate the model
            if (ModelState.IsValid == false)
            {
                result.AddModelStateErrors(ModelState);
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(result);
            }

            // Validate the Captcha
            try
            {
                if (await ValidateRecaptcha(model.Recaptcha) == false)
                    result.Errors.Add(new ApiErrorItem() { ErrorType = ApiErrorType.GeneralFailure, ErrorCode = ApiErrorCode.InvalidCaptcha });
            }
            catch (Exception ex)
            {
                result.Errors.Add(new ApiErrorItem() { ErrorType = ApiErrorType.GeneralFailure, ErrorCode = ApiErrorCode.Generic, Message = ex.Message });
            }


            if (result.HasErrors)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(result);
            }

            // perform the password change
            try
            {
                var distinguishedName = await GetDN(model.Username);

                if (string.IsNullOrEmpty(distinguishedName))
                {
                    result.Errors.Add(new ApiErrorItem() { ErrorType = ApiErrorType.GeneralFailure, ErrorCode = ApiErrorCode.InvalidCredentials, Message = "Invalid Username or Password" });
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                }

                var cn = new LdapConnection();

                await cn.Connect(Settings.PasswordChangeOptions.LdapHostname, Settings.PasswordChangeOptions.LdapPort);
                await cn.Bind(Settings.PasswordChangeOptions.LdapUsername, Settings.PasswordChangeOptions.LdapPassword);
                var modList = new ArrayList();
                var attribute = new LdapAttribute("userPassword", model.NewPassword);
                modList.Add(new LdapModification(LdapModificationOp.Replace, attribute));
                var mods = (LdapModification[])modList.ToArray(typeof(LdapModification));
                await cn.Modify(distinguishedName, mods);
                cn.Disconnect();
            }
            catch (Exception ex)
            {
                result.Errors.Add(new ApiErrorItem() { ErrorType = ApiErrorType.GeneralFailure, ErrorCode = ApiErrorCode.Generic, Message = ex.Message });
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(result);
            }

            if (result.HasErrors)
                Response.StatusCode = (int)HttpStatusCode.BadRequest;

            return Json(result);

        }

        private string GetBase()
        {
            var _base = string.Empty;
            var hostName = Settings.PasswordChangeOptions.LdapHostname.Split('.');

            foreach (var part in hostName)
            {
                if (part == hostName.Last())
                    _base += $"dc={_base}";
                else
                    _base += $"dc={_base},";
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
                var lsc = await cn.Search($"{GetBase()}", LdapConnection.ScopeSub, $"(mail={username})");

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

        //private static UserPrincipal AcquireUserPricipal(PrincipalContext context, string username)
        //{
        //    return UserPrincipal.FindByIdentity(context, username);
        //}

        //private PrincipalContext AcquirePrincipalContext()
        //{
        //    PrincipalContext principalContext = null;
        //    if (Settings.PasswordChangeOptions.UseAutomaticContext)
        //    {
        //        principalContext = new PrincipalContext(ContextType.Domain);
        //    }
        //    else
        //    {
        //        principalContext = new PrincipalContext(
        //            ContextType.Domain,
        //            $"{Settings.PasswordChangeOptions.LdapHostname}:{Settings.PasswordChangeOptions.LdapPort.ToString()}",
        //            Settings.PasswordChangeOptions.LdapUsername,
        //            Settings.PasswordChangeOptions.LdapPassword);
        //    }

        //    return principalContext;
        //}

        public async Task<bool> ValidateRecaptcha(string recaptchaResponse)
        {
            // skip validation if we don't enable recaptcha
            if (string.IsNullOrWhiteSpace(Settings.RecaptchaPrivateKey)) return true;

            // immediately return false because we don't 
            if (string.IsNullOrEmpty(recaptchaResponse)) return false;

            var requestUrl = $"https://www.google.com/recaptcha/api/siteverify?secret={Settings.RecaptchaPrivateKey}&response={recaptchaResponse}";

            using (var client = new HttpClient())
            {
                var response = await client.GetStringAsync(requestUrl);
                dynamic validationResponse = JsonConvert.DeserializeObject(response);
                return validationResponse.success;
            }
        }
    }
}
