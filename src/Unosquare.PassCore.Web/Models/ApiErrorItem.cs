namespace Unosquare.PassCore.Web.Models
{

    public enum ApiErrorType
    {
        Success = 0,
        GeneralFailure = 1,
        FieldValidation = 2,
    }

    public enum ApiErrorCode
    {
        Generic = 0,
        FieldRequired = 1,
        FieldMismatch = 2,
        UserNotFound = 3,
        InvalidCredentials = 4,
        InvalidCaptcha = 5,
    }


    /// <summary>
    /// Defines the fields contained in one of the items of Api Errors
    /// </summary>
    public class ApiErrorItem
    {
        public ApiErrorType ErrorType { get; set; }

        public ApiErrorCode ErrorCode { get; set; }

        public string FieldName { get; set; }

        public string Message { get; set; }

    }
}
