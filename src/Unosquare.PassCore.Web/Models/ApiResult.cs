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
        public ApiResult(object payload = null)
        {
            Errors = new List<ApiErrorItem>();
            Payload = payload;
        }

        /// <summary>
        /// Gets a value indicating whether this instance has errors.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has errors; otherwise, <c>false</c>.
        /// </value>
        public bool HasErrors => Errors.Count > 0;

        /// <summary>
        /// Gets or sets the errors.
        /// </summary>
        public List<ApiErrorItem> Errors { get; }

        /// <summary>
        /// Gets or sets the payload.
        /// </summary>
        public object Payload { get;  }

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
        /// <returns></returns>
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
        /// <returns></returns>
        public static ApiResult FromModelStateErrors(ModelStateDictionary modelState)
        {
            var result = new ApiResult();

            foreach (var state in modelState.Where(x => x.Value.Errors.Any()))
            {
                var error = state.Value.Errors.First();

                if (error.ErrorMessage.Equals(nameof(ApiErrorCode.FieldRequired)))
                    result.AddFieldRequiredValidationError(state.Key);
                else if (error.ErrorMessage.Equals(nameof(ApiErrorCode.FieldMismatch)))
                    result.AddFieldMismatchValidationError(state.Key);
                else
                    result.AddGenericFieldValidationError(state.Key, error.ErrorMessage);
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