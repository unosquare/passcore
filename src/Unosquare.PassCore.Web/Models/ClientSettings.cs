namespace Unosquare.PassCore.Web.Models
{
    /// <summary>
    /// Represents all of the strongly-typed application settings loaded from a JSON file.
    /// </summary>
    public class ClientSettings
    {
        public Alerts Alerts { get; set; }
        public bool ShowPasswordMeter { get; set; }
        public bool UseEmail { get; set; }
        public ChangePasswordForm ChangePasswordForm { get; set; }
        public ErrorsPasswordForm ErrorsPasswordForm { get; set; }
        public Recaptcha Recaptcha { get; set; }
        public string ApplicationTitle { get; set; }
        public string ChangePasswordTitle { get; set; }
        public ValidationRegex ValidationRegex { get; set; }
    }

    public class ChangePasswordForm
    {
        public string ChangePasswordButtonLabel { get; set; }
        public string CurrentPasswordHelpblock { get; set; }
        public string CurrentPasswordLabel { get; set; }
        public string HelpText { get; set; }
        public string HelpTitle { get; set; }
        public string NewPasswordHelpblock { get; set; }
        public string NewPasswordLabel { get; set; }
        public string NewPasswordVerifyHelpblock { get; set; }
        public string NewPasswordVerifyLabel { get; set; }
        public string UsernameDefaultDomainHelperBlock { get; set; }
        public string UsernameHelpblock { get; set; }
        public string UsernameLabel { get; set; }
    }

    public class Recaptcha
    {
        public string LanguageCode { get; set; }
        public string SiteKey { get; set; }
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