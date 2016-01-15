namespace Unosquare.PassCore.Web.Models
{
    using System.Collections.Generic;

    public class AppSettings
    {
        public string RecaptchaPrivateKey { get; set; }
        public ClientSettings ClientSettings { get; set; }
    }

    public class ClientSettings
    {
        public string ApplicationTitle { get; set; }
        public string ChangePasswordTitle { get; set; }
        public ChangePasswordForm ChangePasswordForm { get; set; }
        public List<string> ErrorMessages { get; set; }
        public Recaptcha Recaptcha { get; set; }
        public Alerts Alerts { get; set; }
    }
    public class ChangePasswordForm
    {
        public string HelpTitle { get; set; }
        public string HelpText { get; set; }
        public string UsernameLabel { get; set; }
        public string UsernamePlaceholder { get; set; }
        public string UsernameHelpblock { get; set; }
        public string CurrentPasswordLabel { get; set; }
        public string CurrentPasswordPlaceholder { get; set; }
        public string CurrentPasswordHelpblock { get; set; }
        public string NewPasswordLabel { get; set; }
        public string NewPasswordPlaceholder { get; set; }
        public string NewPasswordHelpblock { get; set; }
        public string NewPasswordVerifyLabel { get; set; }
        public string NewPasswordVerifyPlaceholder { get; set; }
        public string NewPasswordVerifyHelpblock { get; set; }
    }

    public class Recaptcha
    {
        public bool IsEnabled { get; set; }
        public string SiteKey { get; set; }
    }

    public class Alerts
    {
        public string SuccessAlertTitle { get; set; }
        public string SuccessAlertBody { get; set; }
        public string ErrorAlertTitle { get; set; }
        public string ErrorAlertBody { get; set; }
    }


}
