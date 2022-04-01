namespace Unosquare.PassCore.Web.Helpers;

public class DebugAppSettings : IAppSettings
{
    private string? defaultDomain;
    private string[]? ldapHostnames;
    private string? ldapPassword;
    private string? ldapUsername;

    public string DefaultDomain
    {
        get => defaultDomain ?? string.Empty;
        set => defaultDomain = value;
    }
    public int LdapPort { get; set; }
    public string[] LdapHostnames
    {
        get => ldapHostnames ?? new string[] { };
        set => ldapHostnames = value;
    }
    public string LdapPassword
    {
        get => ldapPassword ?? string.Empty;
        set => ldapPassword = value;
    }
    public string LdapUsername
    {
        get => ldapUsername ?? string.Empty;
        set => ldapUsername = value;
    }
}