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
    using Unosquare.PassCore.Web.Helpers;

    /// <summary>
    /// Represents a controller class holding all of the server-side functionality of this tool.
    /// </summary>
    [Route("api/[controller]")]
    public class PasswordController : Controller
    {
        private readonly AppSettings _options;

        private readonly IPasswordChangeProvider _iPasswordChangeProvider;

        public PasswordController(IOptions<AppSettings> optionsAccessor, IPasswordChangeProvider iPasswordChangeProvider)
        {
            _options = optionsAccessor.Value;
            _iPasswordChangeProvider = iPasswordChangeProvider;
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

            var resultPasswordChange = _iPasswordChangeProvider.PerformPasswordChange(model, _options);

            if (resultPasswordChange!=null)
            {
                result.Errors.Add(resultPasswordChange);
            }

            if (result.HasErrors)
                Response.StatusCode = (int)HttpStatusCode.BadRequest;

            return Json(result);
        }

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
