namespace Unosquare.PassCore.Common
{
    using System;

    /// <inheritdoc />
    /// <summary>
    /// Special Exception to transport the ApiErrorItem.
    /// </summary>
    public class ApiErrorException : Exception
    {
        public ApiErrorException(ApiErrorCode errorCode = ApiErrorCode.Generic) 
        {
            ErrorCode = errorCode;
        }

        public ApiErrorException(string message, ApiErrorCode errorCode = ApiErrorCode.Generic)
        : base(message)
        {
            ErrorCode = errorCode;
        }
        
        public ApiErrorCode ErrorCode { get; set; }

        /// <inheritdoc />
        public override string Message => $"Error Code: {ErrorCode}\r\n{base.Message}";

        public ApiErrorItem ToApiErrorItem()
        {
            return new ApiErrorItem
            {
                ErrorCode =  ErrorCode,
                Message = base.Message
            };
        }
    }
}


