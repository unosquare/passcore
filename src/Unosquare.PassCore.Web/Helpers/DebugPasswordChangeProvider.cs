namespace Unosquare.PassCore.Web.Helpers
{
    using System;
    using Common;

    internal class DebugPasswordChangeProvider : IPasswordChangeProvider
    {
        public ApiErrorItem? PerformPasswordChange(string username, string currentPassword, string newPassword)
        {
            var currentUsername = username.IndexOf("@", StringComparison.Ordinal) > 0
                ? username.Substring(0, username.IndexOf("@", StringComparison.Ordinal))
                : username;

            return currentUsername switch
            {
                "error" => new ApiErrorItem(ApiErrorCode.Generic, "Error"),
                "changeNotPermitted" => new ApiErrorItem(ApiErrorCode.ChangeNotPermitted),
                "fieldMismatch" => new ApiErrorItem(ApiErrorCode.FieldMismatch),
                "fieldRequired" => new ApiErrorItem(ApiErrorCode.FieldRequired),
                "invalidCaptcha" => new ApiErrorItem(ApiErrorCode.InvalidCaptcha),
                "invalidCredentials" => new ApiErrorItem(ApiErrorCode.InvalidCredentials),
                "invalidDomain" => new ApiErrorItem(ApiErrorCode.InvalidDomain),
                "userNotFound" => new ApiErrorItem(ApiErrorCode.UserNotFound),
                "ldapProblem" => new ApiErrorItem(ApiErrorCode.LdapProblem),
                _ => null
            };
        }
    }
}
