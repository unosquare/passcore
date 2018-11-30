#pragma warning disable SA1310 // Field names must not contain underscore
namespace Unosquare.PassCore.PasswordProvider
{
    using System;

    /// <summary>
    /// This code is taken from the answer https://stackoverflow.com/a/1766203
    /// from https://stackoverflow.com/questions/1394025/active-directory-ldap-check-account-locked-out-password-expired.
    /// </summary>
    public partial class PasswordChangeProvider
    {
        // See http://support.microsoft.com/kb/155012
        internal const int ErrorPasswordMustChange = 1907;

        // It gives this error if the account is locked, REGARDLESS OF WHETHER VALID CREDENTIALS WERE PROVIDED!!!
        internal const int ErrorPasswordExpired = 1330;

        // here are enums
        internal enum LogonTypes : uint
        {
            /// <summary>
            /// The interactive
            /// </summary>
            Interactive = 2,

            /// <summary>
            /// The network
            /// </summary>
            Network = 3,

            /// <summary>
            /// The service
            /// </summary>
            Service = 5,
        }

        internal enum LogonProviders : uint
        {
            /// <summary>
            /// The default for platform (use this!)
            /// </summary>
            Default = 0,
        }

        [System.Runtime.InteropServices.DllImport("advapi32.dll", SetLastError = true)]
        internal static extern bool LogonUser(
            string principal,
            string authority,
            string password,
            LogonTypes logonType,
            LogonProviders logonProvider,
            out IntPtr token);
    }
}
#pragma warning restore SA1310 // Field names must not contain underscore