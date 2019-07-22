namespace Unosquare.PassCore.PasswordProvider
{
    using System;

    [Flags]
    internal enum PasswordProperties
    {
        DomainPasswordComplex = 0x00000001,
        DomainPasswordNoAnonChange = 0x00000002,
        DomainPasswordNoClearChange = 0x00000004,
        DomainLockoutAdmins = 0x00000008,
        DomainPasswordStoreCleartext = 0x00000010,
        DomainRefusePasswordChange = 0x00000020,
    }
}
