using System;
using System.Collections.Generic;
using System.Text;

namespace Unosquare.PassCore.Common
{
    /// <summary>
    /// Special Exception to transport the ApiErrorItem.
    /// </summary>
    public class ApiErrorException : Exception
    {
        public ApiErrorItem ErrorItem { get; set; }

        public ApiErrorException() 
        {
            // Default ApiErrorItem to prevent Null-PointerException
            ErrorItem = new ApiErrorItem
            {
                ErrorCode = ApiErrorCode.Generic,
                FieldName = "",
                Message = "Some Error"
            };
        }

        /// <summary>
        /// Exception message and/or ApiErrorItem.Message.
        /// </summary>
        public override string Message
        {
            get
            {
                string mess = base.Message;

                if (this.ErrorItem != null)
                {
                    if (!String.IsNullOrWhiteSpace(base.Message))
                    {
                        mess += ": ";
                    }
                    mess += this.ErrorItem.Message;
                }
                return mess;
            }
        }
    }
}


