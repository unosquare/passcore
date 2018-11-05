namespace Unosquare.PassCore.Common
{
    using System;

    /// <inheritdoc />
    /// <summary>
    /// Special Exception to transport the ApiErrorItem.
    /// </summary>
    public class ApiErrorException : Exception
    {
        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Unosquare.PassCore.Common.ApiErrorException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="errorCode">The error code.</param>
        public ApiErrorException(string message, ApiErrorCode errorCode = ApiErrorCode.Generic)
        : base(message)
        {
            ErrorCode = errorCode;
        }

        /// <summary>
        /// Gets or sets the error code.
        /// </summary>
        /// <value>
        /// The error code.
        /// </value>
        public ApiErrorCode ErrorCode { get; }

        /// <inheritdoc />
        public override string Message => $"Error Code: {ErrorCode}\r\n{base.Message}";

        /// <summary>
        /// To the API error item.
        /// </summary>
        /// <returns></returns>
        public ApiErrorItem ToApiErrorItem() => new ApiErrorItem(ErrorCode, base.Message);
    }
}


