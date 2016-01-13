using Microsoft.AspNet.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Unosquare.PassCore.Web.Models
{
    public class ApiResult
    {
        public bool HasErrors { get { return Errors.Count > 0; } }

        public List<ApiErrorItem> Errors { get; set; } = new List<ApiErrorItem>();

        public object Response { get; set; } = null;

        public static ApiResult Success()
        {
            return new ApiResult() { Response = "OK" };
        }

        public static ApiResult InvalidRequest()
        {
            var result = new ApiResult() { Response = "Invalid Request" };
            result.Errors.Add(new ApiErrorItem()
            {
                ErrorCode = ApiErrorCode.Generic,
                ErrorType = ApiErrorType.InvalidRequest,
                FieldName = "",
                Message = "Invalid Request"
            });

            return result;
        }

        public void AddValidationError(string fieldName, ApiErrorCode errorCode)
        {
            this.Errors.Add(new ApiErrorItem()
            {
                ErrorCode = errorCode,
                ErrorType = ApiErrorType.FieldValidation,
                FieldName = fieldName,
                Message = errorCode.ToString()
            });
        }

        public void AddFieldRequiredValidationError(string fieldName)
        {
            this.Errors.Add(new ApiErrorItem()
            {
                ErrorCode = ApiErrorCode.FieldRequired,
                ErrorType = ApiErrorType.FieldValidation,
                FieldName = fieldName,
                Message = nameof(ApiErrorCode.FieldRequired)
            });
        }

        public void AddFieldMismatchValidationError(string fieldName)
        {
            this.Errors.Add(new ApiErrorItem()
            {
                ErrorCode = ApiErrorCode.FieldMismatch,
                ErrorType = ApiErrorType.FieldValidation,
                FieldName = fieldName,
                Message = nameof(ApiErrorCode.FieldMismatch)
            });
        }

        public void AddGenericFieldValidationError(string fieldName, string message)
        {
            this.Errors.Add(new ApiErrorItem()
            {
                ErrorCode = ApiErrorCode.Generic,
                ErrorType = ApiErrorType.FieldValidation,
                FieldName = fieldName,
                Message = message
            });
        }

        public void AddOperationError(ApiErrorCode errorCode, string message)
        {
            this.Errors.Add(new ApiErrorItem()
            {
                ErrorCode = errorCode,
                ErrorType = ApiErrorType.SystemOperation,
                FieldName = string.Empty,
                Message = message
            });
        }

        public void AddModelStateErrors(ModelStateDictionary modelState)
        {
            foreach (var state in modelState)
            {
                if (state.Value.Errors.Any())
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
}
