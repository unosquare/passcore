namespace Unosquare.PassCore.Web.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents all of the strongly-typed application settings loaded from a JSON file
    /// </summary>
    public class AppSettings
    {
        public bool EnableHttpsRedirect { get; set; } = true;
        public string RecaptchaPrivateKey { get; set; }
        public PasswordChangeOptions PasswordChangeOptions { get; set; }
        public ClientSettings ClientSettings { get; set; }
    }

    public class PasswordChangeOptions
    {
        public bool UseAutomaticContext { get; set; } = true;
        public string LdapHostname { get; set; }
        public int LdapPort { get; set; } = 389;
        public string LdapUsername { get; set; }
        public string LdapPassword { get; set; }
    }

    public class ClientSettings
    {
        public string ApplicationTitle { get; set; }
        public string ChangePasswordTitle { get; set; }
        public ChangePasswordForm ChangePasswordForm { get; set; }
        public string DefaultDomain { get; set; }
        public List<string> ErrorMessages { get; set; }
        public ErrorsPasswordForm ErrorsPasswordForm { get; set; }
        public Recaptcha Recaptcha { get; set; }
        public bool ShowPasswordMeter { get; set; }
        public Alerts Alerts { get; set; }
        public bool CheckRestrictedAdGroups { get; set; }
        public List<string> RestrictedADGroups { get; set; }
    }

    public class ChangePasswordForm
    {
        public string HelpTitle { get; set; }
        public string HelpText { get; set; }
        public string UsernameLabel { get; set; }
        public string UsernameHelpblock { get; set; }
        public string CurrentPasswordLabel { get; set; }
        public string CurrentPasswordHelpblock { get; set; }
        public string NewPasswordLabel { get; set; }
        public string NewPasswordHelpblock { get; set; }
        public string NewPasswordVerifyLabel { get; set; }
        public string NewPasswordVerifyHelpblock { get; set; }
        public string ChangePasswordButtonLabel { get; set; }
    }

    public class Recaptcha
    {
        public bool IsEnabled { get; set; }
        public string SiteKey { get; set; }
        public string LanguageCode { get; set; }
    }

    public class Alerts
    {
        public string SuccessAlertTitle { get; set; }
        public string SuccessAlertBody { get; set; }
        public string ErrorAlertTitle { get; set; }
        public string ErrorAlertBody { get; set; }
        public string ErrorPasswordChangeNotAllowed { get; set; }
        public string ErrorInvalidCredentials { get; set; }
    }

    public class ErrorsPasswordForm
    {
        public string FieldRequired { get; set; }
        public string UsernamePattern { get; set; }
        public string UsernameEmailPattern { get; set; }
        public string PasswordMatch { get; set; }
    }
}