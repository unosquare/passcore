namespace Unosquare.PassCore.Common
{
    /// <summary>
    /// Defines the fields contained in one of the items of Api Errors
    /// </summary>
    public class ApiErrorItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApiErrorItem" /> class.
        /// </summary>
        /// <param name="errorCode">The error code.</param>
        /// <param name="message">The message.</param>
        public ApiErrorItem(ApiErrorCode errorCode, string message = null)
        {
            ErrorCode = errorCode;
            Message = message;
        }

        /// <summary>
        /// Gets or sets the error code.
        /// </summary>
        /// <value>
        /// The error code.
        /// </value>
        public ApiErrorCode ErrorCode { get; }

        /// <summary>
        /// Gets or sets the name of the field.
        /// </summary>
        /// <value>
        /// The name of the field.
        /// </value>
        public string FieldName { get; set; }

        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; }
    }
}
