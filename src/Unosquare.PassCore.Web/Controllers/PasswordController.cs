namespace Unosquare.PassCore.Web.Controllers
{
    using Microsoft.AspNet.Mvc;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json;
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using Unosquare.PassCore.Web.Models;
    using System.Net.Http;
    using System.DirectoryServices.AccountManagement;
    using System.Collections.Generic;
    [Route("api/[controller]")]
    public class PasswordController : ControllerBase
    {
        public PasswordController(IConfigurationRoot configuration)
            : base(configuration)
        {
            // placeholder
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Json(Settings.ClientSettings);
        }

        // POST api/password
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]ChangePasswordModel value)
        {

            // Validate the request
            if (value == null)
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
                if (await ValidateRecaptcha(value.Recaptcha) == false)
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
                var principalContext = new PrincipalContext(ContextType.Domain);
                var userPrincipal = UserPrincipal.FindByIdentity(principalContext, value.Username);

                if (userPrincipal == null)
                {
                    result.Errors.Add(new ApiErrorItem() { ErrorType = ApiErrorType.GeneralFailure, ErrorCode = ApiErrorCode.UserNotFound });
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return Json(result);
                }

                userPrincipal.ChangePassword(value.CurrentPassword, value.NewPassword);
                userPrincipal.Save();
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
