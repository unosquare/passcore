namespace Zyborg.PassCore.PasswordProvider.LDAP
{
    using Novell.Directory.Ldap;

    public class LdapPasswordChangeOptions
    {
        /// Required, one or more hostnames or IP addresses which expose an LDAP/LDAPS
        /// service endpoint that will be connected to.  If more than one host is
        /// specified, then each will be tried in turn until a successful, secure
        /// connection is established.
        public string[] LdapHostnames { get; set; }

        /// Optional, defaults to 636 -- the default port for LDAPS (i.e. LDAP over TLS).
        /// A common alternative is to use the default LDAP port, 389, however this port
        /// typically is not-secured and requires the "StartTLS" flag enabled.
        public int LdapPort { get; set; } = LdapConnection.DEFAULT_SSL_PORT;

        /// Optional, if 'true', then the specified port is a non-secured port by default
        /// and requires the use of the "StartTLS" command over LDAP to enable TLS.
        public bool LdapStartTls { get; set; } = false;

        /// Optional, if 'true', then server certificates will be ignored for expiration
        /// or common name mismatch.  Note this is a SUPERSET of the LdapIgnoreTlsValidation
        /// options, so you don't have to set both.
        public bool LdapIgnoreTlsErrors { get; set; } = false;

        /// Optional, if 'true', then server certificates will be accepted regardless
        /// of being signed by a trusted CA or intermediary (e.g. self-signed).
        public bool LdapIgnoreTlsValidation { get; set; } = false;

        /// The distinguished name (DN) of the privileged user account in the
        /// target directory with permission to reset user passwords.
        public string LdapBindUserDN { get; set; }

        /// The password of the privileged user account in the
        /// target directory with permission to reset user passwords.
        public string LdapBindPassword { get; set; }

        /// Distinguished Name (DN) of the base OU from which to search for
        /// the target users by their username (SAM Account Name)
        public string LdapSearchBase { get; set; }

        /// When the user cannot be located in the directory, you can
        /// either expose that error, or hide it and treat like an arbitrary
        /// bad credential -- in order to prevent brute force attack to
        /// discover the presence or absence of a username.
        public bool HideUserNotFound { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether [LDAP change password with delete add].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [LDAP change password with delete add]; otherwise, <c>false</c>.
        /// </value>
        public bool LdapChangePasswordWithDelAdd { get; set; } = true;

        /// <summary>
        /// Gets or sets the LDAP search filter.
        /// </summary>
        /// <value>
        /// The LDAP search filter.
        /// </value>
        public string LdapSearchFilter { get; set; } = "(sAMAccountName={Username})";
    }
}