namespace Unosquare.PassCore.Web.Helpers
{
    using Unosquare.PassCore.Web.Models;

    internal class DebugPasswordChangeProvider : IPasswordChangeProvider
    {
        public ApiErrorItem PerformPasswordChange(ChangePasswordModel model)
        {
            switch (model.Username)
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
