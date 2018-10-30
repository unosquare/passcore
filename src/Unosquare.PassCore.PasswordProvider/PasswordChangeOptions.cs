namespace Unosquare.PassCore.PasswordProvider
{
    using Common;
    using System.Collections.Generic;

    public class PasswordChangeOptions : IAppSettings
    {
        public bool UseAutomaticContext { get; set; } = true;
        public List<string> RestrictedADGroups { get; set; }
        public List<string> AllowedADGroups { get; set; }
        public string IdTypeForUser { get; set; }
        public string RecaptchaPrivateKey { get; set; }
        public string DefaultDomain { get; set; }
        public int LdapPort { get; set; }
        public string[] LdapHostnames { get; set; }
        public string LdapPassword { get; set; }
        public string LdapUsername { get; set; }
        public bool UpdateLastPassword { get; set; }
    }
}
