namespace Unosquare.PassCore.PasswordProvider
{
    using Common;
    using System.Collections.Generic;

    /// <summary>
    /// Represents the options of this provider.
    /// </summary>
    /// <seealso cref="Unosquare.PassCore.Common.IAppSettings" />
    public class PasswordChangeOptions : IAppSettings
    {
        private string? defaultDomain;
        private string? ldapPassword;
        private string[]? ldapHostnames;
        private string? ldapUsername;

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
        public List<string>? RestrictedADGroups { get; set; }

        /// <summary>
        /// Gets or sets the allowed ad groups.
        /// </summary>
        /// <value>
        /// The allowed ad groups.
        /// </value>
        public List<string>? AllowedADGroups { get; set; }

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
            get => defaultDomain ?? string.Empty;
            set => defaultDomain = value;
        }

        /// <inheritdoc />
        public int LdapPort { get; set; }

        /// <inheritdoc />
        public string[] LdapHostnames
        {
            get => ldapHostnames ?? new string[] { };
            set => ldapHostnames = value;
        }

        /// <inheritdoc />
        public string LdapPassword
        {
            get => ldapPassword ?? string.Empty;
            set => ldapPassword = value;
        }

        /// <inheritdoc />
        public string LdapUsername
        {
            get => ldapUsername ?? string.Empty;
            set => ldapUsername = value;
        }
    }
}
