namespace Unosquare.PassCore.Web.Controllers
{
    using System;
    using System.Globalization;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Common;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Models;
    using Newtonsoft.Json;
    using Zxcvbn;

    /// <summary>
    /// Represents a controller class holding all of the server-side functionality of this tool.
    /// </summary>
    [Route("api/[controller]")]
    public class PasswordController : Controller
    {
        private readonly ILogger _logger;
        private readonly ClientSettings _options;
        private readonly string[] _words;
        private readonly IPasswordChangeProvider _passwordChangeProvider;
        private readonly Random _random;

        /// <summary>
        /// Initializes a new instance of the <see cref="PasswordController" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="optionsAccessor">The options accessor.</param>
        /// <param name="wordsAccessor">The words accessor.</param>
        /// <param name="passwordChangeProvider">The password change provider.</param>
        public PasswordController(
            ILogger<PasswordController> logger,
            IOptions<ClientSettings> optionsAccessor,
            IOptions<Words> wordsAccessor,
            IPasswordChangeProvider passwordChangeProvider)
        {
            _logger = logger;
            _options = optionsAccessor.Value;
            _words = wordsAccessor.Value.WordsDictionary;
            _passwordChangeProvider = passwordChangeProvider;

            if (_options.ForcePasswordGeneration) _random = new Random();
        }

        /// <summary>
        /// Returns the ClientSettings object as a JSON string.
        /// </summary>
        /// <returns>A Json representation of the ClientSettings object.</returns>
        [HttpGet]
        public IActionResult Get() => Json(_options);

        /// <summary>
        /// Returns generated password as a JSON string.
        /// </summary>
        /// <returns>A Json with a password property which contains a random generated password.</returns>
        [HttpGet]
        [Route("generated")]
        public IActionResult GetGeneratedPassword()
        {
            var score = 0;
            var password = string.Empty;
            var maxNumber = _words.Length;

            do
            {
                var firstIndex = _random.Next(0, maxNumber);
                var secondIndex = _random.Next(0, maxNumber);

                // if both indexes have the same value, repeat the random
                if (secondIndex == firstIndex) continue;

                var firstWord = _words[firstIndex];
                if (int.Parse(firstIndex.ToString().Substring(0, 1)) < 4)
                    firstWord = char.ToUpper(firstWord[0], CultureInfo.InvariantCulture) + firstWord.Substring(1);

                var secondWord = _words[secondIndex];
                if (int.Parse(secondIndex.ToString().Substring(0, 1)) < 4)
                    secondWord = char.ToUpper(secondWord[0], CultureInfo.InvariantCulture) + secondWord.Substring(1);

                password = $"{firstWord}{secondIndex}_{secondWord}{firstIndex}";
                score = Zxcvbn.MatchPassword(password).Score;
            }
            while (score < 4);

            return Json(new { password });
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
                var resultPasswordChange = _passwordChangeProvider.PerformPasswordChange(
                        model.Username,
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

        private async Task<bool> ValidateRecaptcha(string recaptchaResponse)
        {
            // skip validation if we don't enable recaptcha
            if (string.IsNullOrWhiteSpace(_options.Recaptcha.PrivateKey))
                return true;

            // immediately return false because we don't 
            if (string.IsNullOrEmpty(recaptchaResponse))
                return false;

            var requestUrl = $"https://www.google.com/recaptcha/api/siteverify?secret={_options.Recaptcha.PrivateKey}&response={recaptchaResponse}";

            using (var client = new HttpClient())
            {
                var response = await client.GetStringAsync(requestUrl);
                dynamic validationResponse = JsonConvert.DeserializeObject(response);
                return validationResponse.success;
            }
        }
    }
}
