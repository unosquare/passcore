namespace Unosquare.PassCore.PasswordProvider
{
    using System.Collections.Generic;

    public class PasswordChangeOptions
    {
        public bool UseAutomaticContext { get; set; } = true;
        public int LdapPort { get; set; } = 389;
        public string LdapHostname { get; set; }
        public string LdapPassword { get; set; }
        public string LdapUsername { get; set; }
        public List<string> RestrictedADGroups { get; set; }
        public bool CheckRestrictedAdGroups { get; set; }
        public List<string> AllowedADGroups { get; set; }
        public bool CheckAllowedAdGroups { get; set; }
    }
}
