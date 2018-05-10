namespace Unosquare.PassCore.Web.Models
{
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