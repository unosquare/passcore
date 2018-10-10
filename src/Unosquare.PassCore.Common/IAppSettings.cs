namespace Unosquare.PassCore.Common
{
    /// <summary>
    /// Interface for any Application provider
    /// </summary>
    public interface IAppSettings
    {
        /// <summary>
        /// Gets or sets the recaptcha private key.
        /// </summary>
        /// <value>
        /// The recaptcha private key.
        /// </value>
        string RecaptchaPrivateKey { get; set; }

        /// <summary>
        /// Gets or sets the default domain.
        /// </summary>
        /// <value>
        /// The default domain.
        /// </value>
        string DefaultDomain { get; set; }

        /// <summary>
        /// Gets or sets the LDAP port.
        /// </summary>
        /// <remarks>
        /// Optional, defaults to 636 -- the default port for LDAPS (i.e. LDAP over TLS).
        /// A common alternative is to use the default LDAP port, 389, however this port
        /// typically is not-secured and requires the "StartTLS" flag enabled.
        /// </remarks>
        /// <value>
        /// The LDAP port.
        /// </value>
        int LdapPort { get; set; }

        /// <summary>
        /// Gets or sets the LDAP hostnames.
        /// </summary>
        /// <remarks>
        ///  Required, one or more hostnames or IP addresses which expose an LDAP/LDAPS
        /// service endpoint that will be connected to.  If more than one host is
        /// specified, then each will be tried in turn until a successful, secure
        /// connection is established.
        /// </remarks>
        /// <value>
        /// The LDAP hostnames.
        /// </value>
        string[] LdapHostnames { get; set; }

        /// <summary>
        /// Gets or sets the LDAP password.
        /// </summary>
        /// <value>
        /// The LDAP password.
        /// </value>
        string LdapPassword { get; set; }

        /// <summary>
        /// Gets or sets the LDAP username.
        /// </summary>
        /// <value>
        /// The LDAP username.
        /// </value>
        string LdapUsername { get; set; }
    }
}
