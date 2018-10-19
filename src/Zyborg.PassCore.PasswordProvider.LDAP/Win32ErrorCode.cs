#pragma warning disable SA1117 // Parameters should be on same line or separate lines
namespace Zyborg.PassCore.PasswordProvider.LDAP
{
    using System;
    using System.Collections.Generic;

    public class Win32ErrorCode
    {
        /// Based on
        /// <a href="https://msdn.microsoft.com/en-us/library/cc231199.aspx?f=255&MSPPError=-2147217396">docs.</a>
        /// provides a list of commonly anticipated error codes from a password change request.
        public static readonly IEnumerable<Win32ErrorCode> Codes = new[]
        {
            new Win32ErrorCode(0x00000005, "ERROR_ACCESS_DENIED",
                "Access is denied."),
            new Win32ErrorCode(0x00000056, "ERROR_INVALID_PASSWORD",
                "The specified network password is not correct."),
            new Win32ErrorCode(0x00000523, "ERROR_INVALID_ACCOUNT_NAME",
                "The name provided is not a properly formed account name."),
            new Win32ErrorCode(0x00000524, "ERROR_USER_EXISTS",
                "The specified account already exists."),
            new Win32ErrorCode(0x00000525, "ERROR_NO_SUCH_USER",
                "The specified account does not exist."),
            new Win32ErrorCode(0x0000052B, "ERROR_WRONG_PASSWORD",
                "Unable to update the password. The value provided as the current password is incorrect."),
            new Win32ErrorCode(0x0000052C, "ERROR_ILL_FORMED_PASSWORD",
                "Unable to update the password. The value provided for the new password contains values that are not allowed in passwords."),
            new Win32ErrorCode(0x0000052D, "ERROR_PASSWORD_RESTRICTION",
                "Unable to update the password. The value provided for the new password does not meet the length, complexity, or history requirements of the domain."),
            new Win32ErrorCode(0x0000052E, "ERROR_LOGON_FAILURE",
                "Logon failure: Unknown user name or bad password."),
            new Win32ErrorCode(0x0000052F, "ERROR_ACCOUNT_RESTRICTION",
                "Logon failure: User account restriction. Possible reasons are blank passwords not allowed, logon hour restrictions, or a policy restriction has been enforced."),
            new Win32ErrorCode(0x00000530, "ERROR_INVALID_LOGON_HOURS",
                "Logon failure: Account logon time restriction violation."),
            new Win32ErrorCode(0x00000531, "ERROR_INVALID_WORKSTATION",
                "Logon failure: User not allowed to log on to this computer."),
            new Win32ErrorCode(0x00000532, "ERROR_PASSWORD_EXPIRED",
                "Logon failure: The specified account password has expired."),
            new Win32ErrorCode(0x00000533, "ERROR_ACCOUNT_DISABLED",
                "Logon failure: Account currently disabled."),
            new Win32ErrorCode(0x00000773, "ERROR_PASSWORD_MUST_CHANGE",
                "The user's password must be changed before logging on the first time."),
            new Win32ErrorCode(0x00000774, "ERROR_DOMAIN_CONTROLLER_NOT_FOUND",
                "Could not find the domain controller for this domain."),
            new Win32ErrorCode(0x00000775, "ERROR_ACCOUNT_LOCKED_OUT",
                "The referenced account is currently locked out and cannot be logged on to."),
        };

        private static readonly Dictionary<int, Win32ErrorCode> ErrorByCode =
                new Dictionary<int, Win32ErrorCode>();
        private static readonly Dictionary<string, Win32ErrorCode> ErrorByCodeName =
                new Dictionary<string, Win32ErrorCode>(StringComparer.InvariantCultureIgnoreCase);

        static Win32ErrorCode()
        {
            foreach (var c in Codes)
            {
                ErrorByCode[c.Code] = c;
                ErrorByCodeName[c.CodeName] = c;
            }
        }

        private Win32ErrorCode(int code, string codeName, string desc)
        {
            Code = code;
            CodeName = codeName;
            Description = desc;
        }

        public int Code { get; }
        public string CodeName { get; }
        public string Description { get; }
        
        public static Win32ErrorCode ByCode(int code) =>
            ErrorByCode.TryGetValue(code, out var err) ? err : null;

        public static Win32ErrorCode ByCodeName(string codeName) =>
            ErrorByCodeName.TryGetValue(codeName, out var err) ? err : null;

        public override int GetHashCode() => Code;
        public override bool Equals(object obj) => obj != null
                && obj is Win32ErrorCode err && Code == err.Code;
    }
}
#pragma warning restore SA1117 // Parameters should be on same line or separate lines