namespace Unosquare.PassCore.Web.Models
{
    /// <summary>
    /// Represent error types
    /// </summary>
    public enum ApiErrorType
    {
        Success = 0,
        GeneralFailure = 1,
        FieldValidation = 2,
    }

    /// <summary>
    /// Represents error codes
    /// </summary>
    public enum ApiErrorCode
    {
        Generic = 0,
        FieldRequired = 1,
        FieldMismatch = 2,
        UserNotFound = 3,
        InvalidCredentials = 4,
        InvalidCaptcha = 5,
        ChangeNotPermitted = 6,
        InvalidDomain = 7
    }

    /// <summary>
    /// Defines the fields contained in one of the items of Api Errors
    /// </summary>
    public class ApiErrorItem
    {
        /// <summary>
        /// Gets or sets the type of the error.
        /// </summary>
        /// <value>
        /// The type of the error.
        /// </value>
        public ApiErrorType ErrorType { get; set; }

        /// <summary>
        /// Gets or sets the error code.
        /// </summary>
        /// <value>
        /// The error code.
        /// </value>
        public ApiErrorCode ErrorCode { get; set; }

        /// <summary>
        /// Gets or sets the name of the field.
        /// </summary>
        /// <value>
        /// The name of the field.
        /// </value>
        public string FieldName { get; set; }

        /// <summary>
        /// Gets or sets the extended message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; set; }
    }
}