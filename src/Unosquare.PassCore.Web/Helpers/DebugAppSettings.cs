namespace Unosquare.PassCore.Web.Helpers
{
    using Common;

    public class DebugAppSettings : IAppSettings
    {
        public string RecaptchaPrivateKey { get; set; }
        public string DefaultDomain { get; set; }
        public int LdapPort { get; set; }
        public string[] LdapHostnames { get; set; }
        public string LdapPassword { get; set; }
        public string LdapUsername { get; set; }
    }
}
