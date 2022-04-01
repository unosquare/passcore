using System.Collections.Generic;
using Unosquare.PassCore.Common;

namespace Unosquare.PassCore.PasswordProvider;

/// <summary>
/// Represents the options of this provider.
/// </summary>
/// <seealso cref="Unosquare.PassCore.Common.IAppSettings" />
public class PasswordChangeOptions : IAppSettings
{
    private string? _defaultDomain;
    private string? _ldapPassword;
    private string[]? _ldapHostnames;
    private string? _ldapUsername;

    /// <summary>
    /// Gets or sets a value indicating whether [use automatic context].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [use automatic context]; otherwise, <c>false</c>.
    /// </value>
    public bool UseAutomaticContext { get; set; } = true;

    /// <summary>
    /// Gets or sets the restricted ad groups.
    /// </summary>
    /// <value>
    /// The restricted ad groups.
    /// </value>
    public List<string>? RestrictedAdGroups { get; set; }

    /// <summary>
    /// Gets or sets the allowed ad groups.
    /// </summary>
    /// <value>
    /// The allowed ad groups.
    /// </value>
    public List<string>? AllowedAdGroups { get; set; }

    /// <summary>
    /// Gets or sets the identifier type for user.
    /// </summary>
    /// <value>
    /// The identifier type for user.
    /// </value>
    public string? IdTypeForUser { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [update last password].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [update last password]; otherwise, <c>false</c>.
    /// </value>
    public bool UpdateLastPassword { get; set; }

    /// <inheritdoc />
    public string DefaultDomain
    {
        get => _defaultDomain ?? string.Empty;
        set => _defaultDomain = value;
    }

    /// <inheritdoc />
    public int LdapPort { get; set; }

    /// <inheritdoc />
    public string[] LdapHostnames
    {
        get => _ldapHostnames ?? new string[] { };
        set => _ldapHostnames = value;
    }

    /// <inheritdoc />
    public string LdapPassword
    {
        get => _ldapPassword ?? string.Empty;
        set => _ldapPassword = value;
    }

    /// <inheritdoc />
    public string LdapUsername
    {
        get => _ldapUsername ?? string.Empty;
        set => _ldapUsername = value;
    }
}