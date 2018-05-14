namespace Unosquare.PassCore.Web.Helpers
{
    using System;
    using Models;
    using Microsoft.Extensions.Options;
    using Common;

    // Sonar-Codacy thought we needed a static method here; and suggested dual default nulls was pointless.
    internal class DebugPasswordChangeProvider : IPasswordChangeProvider
    {
        private readonly AppSettings _options;

        public DebugPasswordChangeProvider(IOptions<AppSettings> options)
        {
            _options = options.Value;
        }

        public ApiErrorItem PerformPasswordChange(string username, string currentPassword, string newPassword)
        {
            var currentUsername = username.Substring(0, username.IndexOf("@", StringComparison.Ordinal));

            switch (currentUsername)
            {
                case "error":
                    return new ApiErrorItem { ErrorCode = ApiErrorCode.Generic, Message ="Error" };
                case "notfound":
                    return new ApiErrorItem { ErrorCode = ApiErrorCode.UserNotFound };
                // TODO: Add missing error codes
                default:
                    return null;
            }
        }
    }
}