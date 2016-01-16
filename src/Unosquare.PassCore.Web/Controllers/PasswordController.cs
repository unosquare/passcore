namespace Unosquare.PassCore.Web.Controllers
{
    using Microsoft.AspNet.Mvc;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json;
    using System;
    using System.DirectoryServices.AccountManagement;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Unosquare.PassCore.Web.Models;


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
                var principalContext = new PrincipalContext(ContextType.Domain);
                var userPrincipal = UserPrincipal.FindByIdentity(principalContext, model.Username);

                if (userPrincipal == null)
                {
                    result.Errors.Add(new ApiErrorItem() { ErrorType = ApiErrorType.GeneralFailure, ErrorCode = ApiErrorCode.UserNotFound });
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return Json(result);
                }

                userPrincipal.ChangePassword(model.CurrentPassword, model.NewPassword);
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
