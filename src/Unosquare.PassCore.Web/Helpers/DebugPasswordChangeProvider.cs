namespace Unosquare.PassCore.Web.Helpers
{
    using System.Collections.Generic;
    using Unosquare.PassCore.Web.Models;

    // Sonar-Codacy thought we needed a static method here; and suggested dual default nulls was pointless.
    internal class DebugPasswordChangeProvider : IPasswordChangeProvider
    {
        ApiErrorItem IPasswordChangeProvider.PerformPasswordChange(ChangePasswordModel model)
        {
            return DebugPasswordChangeProvider.PerformPasswordChange(model);
        }

        protected static ApiErrorItem PerformPasswordChange(ChangePasswordModel model)
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
    }
}