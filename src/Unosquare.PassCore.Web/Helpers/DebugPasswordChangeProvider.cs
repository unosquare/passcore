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

            // Even in DEBUG, it is safe to make this call and check the password anyway
            if (PwnedPasswordsSearch.PwnedSearch.IsPwnedPassword(newPassword))
                return new ApiErrorItem(ApiErrorCode.PwnedPassword);

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
                "pwnedPassword" => new ApiErrorItem(ApiErrorCode.PwnedPassword),
                _ => null
            };
        }
    }
}
