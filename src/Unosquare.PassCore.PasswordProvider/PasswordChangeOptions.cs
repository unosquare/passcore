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
        public AdGroups AdGroups { get; set; }
    }

    public class AdGroups
    {
        public bool CheckRestricted { get; set; }
        public List<string> Restricted { get; set; }
        public bool CheckAllowed { get; set; }
        public List<string> Allowed { get; set; }
    }
}
