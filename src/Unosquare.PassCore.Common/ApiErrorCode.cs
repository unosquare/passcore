namespace Unosquare.PassCore.Common
{
    /// <summary>
    /// Represents error codes
    /// </summary>
    public enum ApiErrorCode
    {
        Generic = 0,
        FieldRequired = 1,
        FieldMismatch = 2,
        UserNotFound = 3,
        InvalidCredentials = 4,
        InvalidCaptcha = 5,
        ChangeNotPermitted = 6,
        InvalidDomain = 7
    }
}