namespace Unosquare.PassCore.PasswordProvider
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct DomainPasswordInformation
    {
        public short MinPasswordLength;
        public short PasswordHistoryLength;
        public PasswordProperties PasswordProperties;
        private long _maxPasswordAge;
        private long _minPasswordAge;

        public TimeSpan MinPasswordAge
        {
            get => -new TimeSpan(_minPasswordAge);
            set => _minPasswordAge = value.Ticks;
        }

        public TimeSpan MaxPasswordAge
        {
            get => -new TimeSpan(_maxPasswordAge);
            set => _maxPasswordAge = value.Ticks;
        }
    }
}
