namespace Unosquare.PassCore.Web.Helpers
{
    using System.Collections.Generic;
    using Unosquare.PassCore.Web.Models;

    // Codacy kept thinking the original implementation was an issue.
    // https://stackoverflow.com/questions/9415257/how-can-i-implement-static-methods-on-an-interface

    internal class DebugPasswordChangeProvider : IPasswordChangeProvider
    {
        static ApiErrorItem PerformPasswordChange(ChangePasswordModel model)
        {
            var username = model.Username.Substring(0, model.Username.IndexOf("@"));
            switch (username)
            {
                case "error":
                    return new ApiErrorItem { ErrorType = ApiErrorType.GeneralFailure, ErrorCode = ApiErrorCode.Generic, Message = "Error" };
                case "notfound":
                    return new ApiErrorItem { ErrorType = ApiErrorType.GeneralFailure, ErrorCode = ApiErrorCode.UserNotFound, Message = "Invalid Username or Password" };
                default:
                    return null;
            }
        }
        ApiErrorItem IPasswordChangeProvider.PerformPasswordChange(ChangePasswordModel model)
        {
            return DebugPasswordChangeProvider.PerformPasswordChange(model);
        }
    }
}
