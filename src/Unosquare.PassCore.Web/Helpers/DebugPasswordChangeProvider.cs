namespace Unosquare.PassCore.Web.Helpers
{
    using System;
    using Models;

    // Sonar-Codacy thought we needed a static method here; and suggested dual default nulls was pointless.
    internal class DebugPasswordChangeProvider : IPasswordChangeProvider
    {
        ApiErrorItem IPasswordChangeProvider.PerformPasswordChange(ChangePasswordModel model)
        {
            var username = model.Username.Substring(0, model.Username.IndexOf("@", StringComparison.Ordinal));

            switch (username)
            {
                case "error":
                    return new ApiErrorItem { ErrorCode = ApiErrorCode.Generic, Message = "Error" };
                case "notfound":
                    return new ApiErrorItem { ErrorCode = ApiErrorCode.UserNotFound, Message = "Invalid Username or Password" };
                default:
                    return null;
            }
        }
    }
}