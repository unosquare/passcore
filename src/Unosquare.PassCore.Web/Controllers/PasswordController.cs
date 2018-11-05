namespace Unosquare.PassCore.Web.Controllers
{
    using Common;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Models;
    using Newtonsoft.Json;
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a controller class holding all of the server-side functionality of this tool.
    /// </summary>
    [Route("api/[controller]")]
    public class PasswordController : Controller
    {
        private readonly ILogger _logger;
        private readonly ClientSettings _options;
        private readonly IPasswordChangeProvider _passwordChangeProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="PasswordController" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="optionsAccessor">The options accessor.</param>
        /// <param name="passwordChangeProvider">The password change provider.</param>
        public PasswordController(
            ILogger<PasswordController> logger,
            IOptions<ClientSettings> optionsAccessor,
            IPasswordChangeProvider passwordChangeProvider)
        {
            _logger = logger;
            _options = optionsAccessor.Value;
            _passwordChangeProvider = passwordChangeProvider;
        }

        /// <summary>
        /// Returns the ClientSettings object as a JSON string.
        /// </summary>
        /// <returns>A Json representation of the ClientSettings object.</returns>
        [HttpGet]
        public IActionResult Get() => Json(_options);

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
                _logger.LogWarning("Null model");

                return BadRequest(ApiResult.InvalidRequest());
            }

            if (model.NewPassword != model.NewPasswordVerify)
            {
                _logger.LogWarning("Invalid model, passwords don't match");

                return BadRequest(ApiResult.InvalidRequest());
            }

            // Validate the model
            if (ModelState.IsValid == false)
            {
                _logger.LogWarning("Invalid model, validation failed");

                return BadRequest(ApiResult.FromModelStateErrors(ModelState));
            }

            // Validate the Captcha
            try
            {
                if (await ValidateRecaptcha(model.Recaptcha).ConfigureAwait(false) == false)
                    throw new InvalidOperationException("Invalid Recaptcha response");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Invalid Recaptcha");
                return BadRequest(ApiResult.InvalidCaptcha());
            }

            var result = new ApiResult();

            try
            {
                var currentUsername = GetUserName(model);
                var resultPasswordChange = _passwordChangeProvider.PerformPasswordChange(
                        currentUsername,
                        model.CurrentPassword,
                        model.NewPassword);

                if (resultPasswordChange == null) return Json(result);
                result.Errors.Add(resultPasswordChange);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update password");

                result.Errors.Add(new ApiErrorItem(ApiErrorCode.Generic, ex.Message));
            }

            return BadRequest(result);
        }

        private string GetUserName(ChangePasswordModel model)
        {
            // Check for default domain: if none given, ensure EFLD can be used as an override.
            var parts = model.Username.Split(new[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
            var domain = parts.Length > 1 ? parts[1] : _passwordChangeProvider.Settings.DefaultDomain;

            return string.IsNullOrWhiteSpace(domain) || parts.Length > 1 ? model.Username : $"{model.Username}@{domain}";
        }

        private async Task<bool> ValidateRecaptcha(string recaptchaResponse)
        {
            // skip validation if we don't enable recaptcha
            if (string.IsNullOrWhiteSpace(_passwordChangeProvider.Settings.RecaptchaPrivateKey))
                return true;

            // immediately return false because we don't 
            if (string.IsNullOrEmpty(recaptchaResponse))
                return false;

            var requestUrl = $"https://www.google.com/recaptcha/api/siteverify?secret={_passwordChangeProvider.Settings.RecaptchaPrivateKey}&response={recaptchaResponse}";

            using (var client = new HttpClient())
            {
                var response = await client.GetStringAsync(requestUrl);
                dynamic validationResponse = JsonConvert.DeserializeObject(response);
                return validationResponse.success;
            }
        }
    }
}