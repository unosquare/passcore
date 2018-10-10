﻿namespace Unosquare.PassCore.Web.Helpers
{
    using System;
    using Microsoft.Extensions.Options;
    using Common;

    // Sonar-Codacy thought we needed a static method here; and suggested dual default nulls was pointless.
    internal class DebugPasswordChangeProvider : IPasswordChangeProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DebugPasswordChangeProvider"/> class.
        /// </summary>
        /// <param name="optionsAccessor">The options accessor.</param>
        public DebugPasswordChangeProvider(IOptions<DebugAppSettings> optionsAccessor)
        {
            Settings = optionsAccessor.Value;
        }
        
        /// <inheritdoc />
        public IAppSettings Settings { get; }

        /// <inheritdoc />
        public ApiErrorItem PerformPasswordChange(string username, string currentPassword, string newPassword)
        {
            var currentUsername = username.Substring(0, username.IndexOf("@", StringComparison.Ordinal));

            switch (currentUsername)
            {
                case "error":
                    return new ApiErrorItem { ErrorCode = ApiErrorCode.Generic, Message = "Error" };
                case "changeNotPermitted":
                    return new ApiErrorItem { ErrorCode = ApiErrorCode.ChangeNotPermitted };
                case "fieldMismatch":
                    return new ApiErrorItem { ErrorCode = ApiErrorCode.FieldMismatch };
                case "fieldRequired":
                    return new ApiErrorItem { ErrorCode = ApiErrorCode.FieldRequired };
                case "invalidCaptcha":
                    return new ApiErrorItem { ErrorCode = ApiErrorCode.InvalidCaptcha };
                case "invalidCredentials":
                    return new ApiErrorItem { ErrorCode = ApiErrorCode.InvalidCredentials };
                case "invalidDomain":
                    return new ApiErrorItem { ErrorCode = ApiErrorCode.InvalidDomain };
                case "userNotFound":
                    return new ApiErrorItem { ErrorCode = ApiErrorCode.UserNotFound };
                default:
                    return null;
            }
        }
    }
}
