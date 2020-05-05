namespace Unosquare.PassCore.Web.Models
{
    using System.Text.Json.Serialization;

    /// <summary>
    /// Represents all of the strongly-typed application settings loaded from a JSON file.
    /// </summary>
    public class ClientSettings
    {
        public Alerts Alerts { get; set; }
        public bool UsePasswordGeneration { get; set; }
        public int MinimumDistance { get; set; }
        public int PasswordEntropy { get; set; }
        public int MinimumScore { get; set; }
        public bool ShowPasswordMeter { get; set; }
        public bool UseEmail { get; set; }
        public ChangePasswordForm ChangePasswordForm { get; set; }
        public ErrorsPasswordForm ErrorsPasswordForm { get; set; }
        public Recaptcha Recaptcha { get; set; }
        public string ApplicationTitle { get; set; }
        public string ChangePasswordTitle { get; set; }
        public ValidationRegex ValidationRegex { get; set; }
    }

    public class Recaptcha
    {
        public string LanguageCode { get; set; }
        public string SiteKey { get; set; }

        [JsonIgnore]
        public string PrivateKey { get; set; }
    }

    public class Alerts
    {
        public string ErrorInvalidCredentials { get; set; }
        public string ErrorInvalidDomain { get; set; }
        public string ErrorPasswordChangeNotAllowed { get; set; }
        public string SuccessAlertBody { get; set; }
        public string SuccessAlertTitle { get; set; }
        public string ErrorInvalidUser { get; set; }
        public string ErrorCaptcha { get; set; }
        public string ErrorFieldRequired { get; set; }
        public string ErrorFieldMismatch { get; set; }
        public string ErrorComplexPassword { get; set; }
        public string ErrorConnectionLdap { get; set; }
        public string ErrorScorePassword { get; set; }
        public string ErrorDistancePassword { get; set; }
    }

    public class ErrorsPasswordForm
    {
        public string FieldRequired { get; set; }
        public string PasswordMatch { get; set; }
        public string UsernameEmailPattern { get; set; }
        public string UsernamePattern { get; set; }
    }

    public class ValidationRegex
    {
        public string EmailRegex { get; set; }
        public string UsernameRegex { get; set; }
    }
}