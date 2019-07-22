namespace Unosquare.PassCore.PasswordProvider
{
    using System;
    using System.Runtime.InteropServices;

    internal sealed partial class SamServer
    {
        [StructLayout(LayoutKind.Sequential)]
        private sealed class UnicodeString : IDisposable
        {
            private ushort Length;
            private ushort MaximumLength;
            public IntPtr Buffer;

            public UnicodeString()
                : this(null)
            {
            }

            public UnicodeString(string s)
            {
                if (s == null) return;
                Length = (ushort)(s.Length * 2);
                MaximumLength = (ushort)(Length + 2);
                Buffer = Marshal.StringToHGlobalUni(s);
            }

            ~UnicodeString() => Dispose(false);

            public override string ToString() => Buffer != IntPtr.Zero ? Marshal.PtrToStringUni(Buffer) : null;

            /// <inheritdoc />
            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            private void Dispose(bool disposing)
            {
                if (Buffer == IntPtr.Zero) return;

                try
                {
                    Marshal.FreeHGlobal(Buffer);
                    Buffer = IntPtr.Zero;
                }
                catch
                {
                    // Ignore
                }
            }
        }
    }
}
