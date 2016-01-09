namespace Unosquare.PassCore.Web.Models
{
    public class AppSettings
    {
        public string ActiveDirectoryRootPath { get; set; } = "LDAP://uno-dc-01.ad.unosquare.com";

        public bool EnablePasswordRecovery { get; set; } = true;
    }
}
