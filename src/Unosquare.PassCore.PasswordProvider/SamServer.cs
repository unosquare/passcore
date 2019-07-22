namespace Unosquare.PassCore.PasswordProvider
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System.Security.Principal;

    internal sealed partial class SamServer : IDisposable
    {
        private const string SamLib = "samlib.dll";
        private IntPtr _handle;

        public SamServer(ServerAccessMask access = ServerAccessMask.SamServerEnumerateDomains | ServerAccessMask.SamServerLookupDomain)
        {
            Check(SamConnect(new UnicodeString(Name), out _handle, access, IntPtr.Zero));
        }

        public string Name { get; } = ".";

        public void Dispose()
        {
            if (_handle == IntPtr.Zero) return;
            SamCloseHandle(_handle);
            _handle = IntPtr.Zero;
        }

        public DomainPasswordInformation GetDomainPasswordInformation(SecurityIdentifier domainSid)
        {
            if (domainSid == null)
                throw new ArgumentNullException(nameof(domainSid));

            var sid = new byte[domainSid.BinaryLength];
            domainSid.GetBinaryForm(sid, 0);

            Check(SamOpenDomain(_handle, DomainAccessMask.DomainReadPasswordParameters, sid, out var domain));
            var info = IntPtr.Zero;

            try
            {
                Check(SamQueryInformationDomain(domain, DomainInformationClass.DomainPasswordInformation, out info));
                return Marshal.PtrToStructure<DomainPasswordInformation>(info);
            }
            finally
            {
                SamFreeMemory(info);
                SamCloseHandle(domain);
            }
        }

        public SecurityIdentifier GetDomainSid(string domain)
        {
            if (domain == null)
                throw new ArgumentNullException(nameof(domain));

            Check(SamLookupDomainInSamServer(_handle, new UnicodeString(domain), out var sid));
            return new SecurityIdentifier(sid);
        }

        public IEnumerable<string> EnumerateDomains()
        {
            var cookie = 0;
            while (true)
            {
                var status = SamEnumerateDomainsInSamServer(_handle, ref cookie, out var info, 1, out var count);
                if (status != NtStatus.StatusSuccess && status != NtStatus.StatusMoreEntries)
                    Check(status);

                if (count == 0)
                    break;

                var us = (UnicodeString)Marshal.PtrToStructure(info + IntPtr.Size, typeof(UnicodeString));
                SamFreeMemory(info);
                yield return us.ToString();
                us.Buffer = IntPtr.Zero; // we don't own this one
            }
        }

        private enum DomainInformationClass
        {
            DomainPasswordInformation = 1,
        }

        [Flags]
        private enum DomainAccessMask
        {
            DomainReadPasswordParameters = 0x00000001,
            DomainWritePasswordParams = 0x00000002,
            DomainReadOtherParameters = 0x00000004,
            DomainWriteOtherParameters = 0x00000008,
            DomainCreateUser = 0x00000010,
            DomainCreateGroup = 0x00000020,
            DomainCreateAlias = 0x00000040,
            DomainGetAliasMembership = 0x00000080,
            DomainListAccounts = 0x00000100,
            DomainLookup = 0x00000200,
            DomainAdministerServer = 0x00000400,
            DomainAllAccess = 0x000F07FF,
            DomainRead = 0x00020084,
            DomainWrite = 0x0002047A,
            DomainExecute = 0x00020301,
        }

        [Flags]
        internal enum ServerAccessMask
        {
            SamServerConnect = 0x00000001,
            SamServerShutdown = 0x00000002,
            SamServerInitialize = 0x00000004,
            SamServerCreateDomain = 0x00000008,
            SamServerEnumerateDomains = 0x00000010,
            SamServerLookupDomain = 0x00000020,
            SamServerAllAccess = 0x000F003F,
            SamServerRead = 0x00020010,
            SamServerWrite = 0x0002000E,
            SamServerExecute = 0x00020021,
        }

        private static void Check(NtStatus err)
        {
            if (err != NtStatus.StatusSuccess)
                throw new Win32Exception($"Error {err} (0x{(int)err:X8})");
        }

        private enum NtStatus
        {
            StatusSuccess = 0x0,
            StatusMoreEntries = 0x105,
            StatusInvalidHandle = unchecked((int)0xC0000008),
            StatusInvalidParameter = unchecked((int)0xC000000D),
            StatusAccessDenied = unchecked((int)0xC0000022),
            StatusObjectTypeMismatch = unchecked((int)0xC0000024),
            StatusNoSuchDomain = unchecked((int)0xC00000DF),
        }

        [DllImport(SamLib, CharSet = CharSet.Unicode)]
        private static extern NtStatus SamConnect(UnicodeString serverName, out IntPtr serverHandle, ServerAccessMask desiredAccess, IntPtr objectAttributes);

        [DllImport(SamLib, CharSet = CharSet.Unicode)]
        private static extern NtStatus SamCloseHandle(IntPtr serverHandle);

        [DllImport(SamLib, CharSet = CharSet.Unicode)]
        private static extern NtStatus SamFreeMemory(IntPtr handle);

        [DllImport(SamLib, CharSet = CharSet.Unicode)]
        private static extern NtStatus SamOpenDomain(IntPtr serverHandle, DomainAccessMask desiredAccess, byte[] domainId, out IntPtr domainHandle);

        [DllImport(SamLib, CharSet = CharSet.Unicode)]
        private static extern NtStatus SamLookupDomainInSamServer(IntPtr serverHandle, UnicodeString name, out IntPtr domainId);

        [DllImport(SamLib, CharSet = CharSet.Unicode)]
        private static extern NtStatus SamQueryInformationDomain(IntPtr domainHandle, DomainInformationClass domainInformationClass, out IntPtr buffer);

        [DllImport(SamLib, CharSet = CharSet.Unicode)]
        private static extern NtStatus SamEnumerateDomainsInSamServer(IntPtr serverHandle, ref int enumerationContext, out IntPtr enumerationBuffer, int preferedMaximumLength, out int countReturned);
    }
}
