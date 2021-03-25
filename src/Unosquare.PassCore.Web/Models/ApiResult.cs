namespace Unosquare.PassCore.Web.Models
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Common;

    /// <summary>
    /// Represent a generic response from a REST API call.
    /// </summary>
    public class ApiResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApiResult"/> class.
        /// </summary>
        /// <param name="payload">The payload.</param>
        public ApiResult(object? payload = null)
        {
            Errors = new List<ApiErrorItem>();
            Payload = payload;
        }

        /// <summary>
        /// Gets or sets the errors.
        /// </summary>
        public List<ApiErrorItem> Errors { get; }

        /// <summary>
        /// Gets or sets the payload.
        /// </summary>
        public object? Payload { get; }

        /// <summary>
        /// Creates a generic invalid request response.
        /// </summary>
        /// <returns>The ApiResult wih Invalid request error.</returns>
        public static ApiResult InvalidRequest()
        {
            var result = new ApiResult("Invalid Request");
            result.Errors.Add(new ApiErrorItem(ApiErrorCode.Generic, "Invalid Request"));

            return result;
        }

        /// <summary>
        /// Invalids the captcha.
        /// </summary>
        /// <returns>The ApiResult from Invalid Recaptcha.</returns>
        public static ApiResult InvalidCaptcha()
        {
            var result = new ApiResult("Invalid Recaptcha");
            result.Errors.Add(new ApiErrorItem(ApiErrorCode.InvalidCaptcha));

            return result;
        }

        /// <summary>
        /// Adds the model state errors.
        /// </summary>
        /// <param name="modelState">State of the model.</param>
        /// <returns>The ApiResult from Model State.</returns>
        public static ApiResult FromModelStateErrors(ModelStateDictionary modelState)
        {
            var result = new ApiResult();

            foreach (var (key, value) in modelState.Where(x => x.Value.Errors.Any()))
            {
                var error = value.Errors.First();

                switch (error.ErrorMessage)
                {
                    case nameof(ApiErrorCode.FieldRequired):
                        result.AddFieldRequiredValidationError(key);
                        break;
                    case nameof(ApiErrorCode.FieldMismatch):
                        result.AddFieldMismatchValidationError(key);
                        break;
                    default:
                        result.AddGenericFieldValidationError(key, error.ErrorMessage);
                        break;
                }
            }

            return result;
        }

        /// <summary>
        /// Adds the field required validation error.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        private void AddFieldRequiredValidationError(string fieldName)
        {
            Errors.Add(new ApiErrorItem(ApiErrorCode.FieldRequired, nameof(ApiErrorCode.FieldRequired))
            {
                FieldName = fieldName,
            });
        }

        /// <summary>
        /// Adds the field mismatch validation error.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        private void AddFieldMismatchValidationError(string fieldName)
        {
            Errors.Add(new ApiErrorItem(ApiErrorCode.FieldMismatch, nameof(ApiErrorCode.FieldMismatch))
            {
                FieldName = fieldName,
            });
        }

        /// <summary>
        /// Adds the generic field validation error.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="message">The message.</param>
        private void AddGenericFieldValidationError(string fieldName, string message)
        {
            Errors.Add(new ApiErrorItem(ApiErrorCode.Generic, message)
            {
                FieldName = fieldName,
            });
        }
    }
}