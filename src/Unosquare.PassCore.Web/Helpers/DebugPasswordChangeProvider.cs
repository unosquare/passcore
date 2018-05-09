namespace Unosquare.PassCore.Web.Helpers
{
    using System.Collections.Generic;
    using Unosquare.PassCore.Web.Models;

    internal class DebugPasswordChangeProvider : IPasswordChangeProvider
    {
        public ApiErrorItem PerformPasswordChange(ChangePasswordModel model)
        {
            var username = model.Username.Substring(0, model.Username.IndexOf("@"));

            switch (username)
            {
                case "pass":
                    return null;
                case "error":
                    return new ApiErrorItem { ErrorType = ApiErrorType.GeneralFailure, ErrorCode = ApiErrorCode.Generic, Message = "Error" };
                case "notfound":
                    return new ApiErrorItem { ErrorType = ApiErrorType.GeneralFailure, ErrorCode = ApiErrorCode.UserNotFound, Message = "Invalid Username or Password" };
                default:
                    return null;
            }
        }
    }
}
