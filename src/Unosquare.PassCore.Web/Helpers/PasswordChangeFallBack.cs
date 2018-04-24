#pragma warning disable SA1310 // Field names must not contain underscore
namespace Unosquare.PassCore.Web.Helpers {
    using System;

    /// <summary>
    /// This code is taken from the answer https://stackoverflow.com/a/1766203 from https://stackoverflow.com/questions/1394025/active-directory-ldap-check-account-locked-out-password-expired
    /// </summary>
    internal static class PasswordChangeFallBack {
        // See http://support.microsoft.com/kb/155012
        internal const int ERROR_PASSWORD_MUST_CHANGE = 1907;
        internal const int ERROR_LOGON_FAILURE = 1326;
        internal const int ERROR_ACCOUNT_RESTRICTION = 1327;
        internal const int ERROR_ACCOUNT_DISABLED = 1331;
        internal const int ERROR_INVALID_LOGON_HOURS = 1328;
        internal const int ERROR_NO_LOGON_SERVERS = 1311;
        internal const int ERROR_INVALID_WORKSTATION = 1329;

        // It gives this error if the account is locked, REGARDLESS OF WHETHER VALID CREDENTIALS WERE PROVIDED!!!
        internal const int ERROR_ACCOUNT_LOCKED_OUT = 1909;
        internal const int ERROR_ACCOUNT_EXPIRED = 1793;
        internal const int ERROR_PASSWORD_EXPIRED = 1330;

        // here are enums
        internal enum LogonTypes : uint {
            Interactive = 2,
            Network = 3,
            Batch = 4,
            Service = 5,
            Unlock = 7,
            NetworkCleartext = 8,
            NewCredentials = 9
        }

        internal enum LogonProviders : uint {
            Default = 0, // default for platform (use this!)
            WinNT35, // sends smoke signals to authority
            WinNT40, // uses NTLM
            WinNT50 // negotiates Kerb or NTLM
        }

        [System.Runtime.InteropServices.DllImport ("advapi32.dll", SetLastError = true)]
        internal static extern bool LogonUser (
            string principal,
            string authority,
            string password,
            LogonTypes logonType,
            LogonProviders logonProvider,
            out IntPtr token);

        [System.Runtime.InteropServices.DllImport ("kernel32.dll", SetLastError = true)]
        internal static extern bool CloseHandle (IntPtr handle);
    }
}
#pragma warning restore SA1310 // Field names must not contain underscore