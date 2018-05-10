namespace Unosquare.PassCore.Web.Helpers
{
    using System;
    using Models;
    using Microsoft.Extensions.Options;

    // Sonar-Codacy thought we needed a static method here; and suggested dual default nulls was pointless.
    internal class DebugPasswordChangeProvider : IPasswordChangeProvider
    {
        private readonly AppSettings _options;

        public DebugPasswordChangeProvider(IOptions<AppSettings> options)
        {
            _options = options.Value;
        }

        public ApiErrorItem PerformPasswordChange(ChangePasswordModel model)
        {
            var username = model.Username.Substring(0, model.Username.IndexOf("@", StringComparison.Ordinal));

            switch (username)
            {
                case "error":
                    return new ApiErrorItem { ErrorCode = ApiErrorCode.Generic, Message = _options.ClientSettings.Alerts.ErrorCaptcha };
                case "notfound":
                    return new ApiErrorItem { ErrorCode = ApiErrorCode.UserNotFound, Message = _options.ClientSettings.Alerts.ErrorInvalidUser };
                default:
                    return null;
            }
        }
    }
}