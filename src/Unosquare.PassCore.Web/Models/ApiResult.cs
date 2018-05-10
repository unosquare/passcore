namespace Unosquare.PassCore.Web.Models
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc.ModelBinding;

    /// <summary>
    /// Represent a generic response from a REST API call
    /// </summary>
    public class ApiResult
    {
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
        public List<ApiErrorItem> Errors { get; set; } = new List<ApiErrorItem>();

        /// <summary>
        /// Gets or sets the payload.
        /// </summary>
        public object Payload { get; set; }

        /// <summary>
        /// Creates a generic invalid request response
        /// </summary>
        /// <returns>The ApiResult wih Invalid request error</returns>
        public static ApiResult InvalidRequest()
        {
            var result = new ApiResult {Payload = "Invalid Request"};
            result.Errors.Add(new ApiErrorItem
            {
                ErrorCode = ApiErrorCode.Generic,
                FieldName = string.Empty,
                Message = "Invalid Request"
            });

            return result;
        }

        /// <summary>
        /// Adds the field required validation error.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        public void AddFieldRequiredValidationError(string fieldName)
        {
            Errors.Add(new ApiErrorItem
            {
                ErrorCode = ApiErrorCode.FieldRequired,
                FieldName = fieldName,
                Message = nameof(ApiErrorCode.FieldRequired)
            });
        }

        /// <summary>
        /// Adds the field mismatch validation error.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        public void AddFieldMismatchValidationError(string fieldName)
        {
            Errors.Add(new ApiErrorItem
            {
                ErrorCode = ApiErrorCode.FieldMismatch,
                FieldName = fieldName,
                Message = nameof(ApiErrorCode.FieldMismatch)
            });
        }

        /// <summary>
        /// Adds the generic field validation error.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="message">The message.</param>
        public void AddGenericFieldValidationError(string fieldName, string message)
        {
            Errors.Add(new ApiErrorItem
            {
                ErrorCode = ApiErrorCode.Generic,
                FieldName = fieldName,
                Message = message
            });
        }

        /// <summary>
        /// Adds the model state errors.
        /// </summary>
        /// <param name="modelState">State of the model.</param>
        public void AddModelStateErrors(ModelStateDictionary modelState)
        {
            foreach (var state in modelState.Where(x => x.Value.Errors.Any()))
            {
                var error = state.Value.Errors.First();

                if (error.ErrorMessage.Equals(nameof(ApiErrorCode.FieldRequired)))
                    AddFieldRequiredValidationError(state.Key);
                else if (error.ErrorMessage.Equals(nameof(ApiErrorCode.FieldMismatch)))
                    AddFieldMismatchValidationError(state.Key);
                else
                    AddGenericFieldValidationError(state.Key, error.ErrorMessage);
            }
        }
    }
}