namespace Unosquare.PassCore.Web.Controllers
{
    using System.Net.Http;
    using System.Net;
    using System.Threading.Tasks;
    using System;
    using Common;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Models;
    using Newtonsoft.Json;

    /// <summary>
    /// Represents a controller class holding all of the server-side functionality of this tool.
    /// </summary>
    [Route("api/[controller]")]
    public class PasswordController : Controller
    {
        private readonly AppSettings _options;
        private readonly IPasswordChangeProvider _passwordChangeProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="PasswordController"/> class.
        /// </summary>
        /// <param name="optionsAccessor">The options accessor.</param>
        /// <param name="passwordChangeProvider">The password change provider.</param>
        public PasswordController(IOptions<AppSettings> optionsAccessor, IPasswordChangeProvider passwordChangeProvider)
        {
            _options = optionsAccessor.Value;
            _passwordChangeProvider = passwordChangeProvider;
        }

        /// <summary>
        /// Returns the ClientSettings object as a JSON string
        /// </summary>
        /// <returns>A Json representation of the ClientSettings object.</returns>
        [HttpGet]
        public IActionResult Get()
        {
            return Json(_options.ClientSettings);
        }

        /// <summary>
        /// Given a POST request, processes and changes a User's password.
        /// </summary>
        /// <param name="model">The value.</param>
        /// <returns>A task representing the async operation.</returns>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ChangePasswordModel model)
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
                // Sonar-Codacy suggested ConfigureAwait
                if (await ValidateRecaptcha(model.Recaptcha).ConfigureAwait(false) == false)
                {
                    result.Errors.Add(new ApiErrorItem { ErrorCode = ApiErrorCode.InvalidCaptcha });
                }
            }
            catch (Exception ex)
            {
                result.Errors.Add(new ApiErrorItem
                {
                    ErrorCode = ApiErrorCode.Generic,
                    Message = ex.Message
                });
            }

            if (result.HasErrors)
            {
                return BadRequest(result);
            }

            var currentUsername = GetUserName(model, result);

            if (result.HasErrors)
            {
                return BadRequest(result);
            }

            var resultPasswordChange = _passwordChangeProvider.PerformPasswordChange(currentUsername, model.CurrentPassword, model.NewPassword);

            if (resultPasswordChange != null)
            {
                result.Errors.Add(resultPasswordChange);
            }

            if (result.HasErrors)
                Response.StatusCode = (int)HttpStatusCode.BadRequest;

            return Json(result);
        }

        private string GetUserName(ChangePasswordModel model, ApiResult result)
        {
            // Check for default domain: if none given, ensure EFLD can be used as an override.
            var parts = model.Username.Split(new[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
            var domain = parts.Length > 1 ? parts[1] : _options.ClientSettings.DefaultDomain;

            // Domain-determinance
            if (string.IsNullOrWhiteSpace(domain))
            {
                return model.Username;
            }

            return parts.Length > 1 ? model.Username : $"{model.Username}@{domain}";
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