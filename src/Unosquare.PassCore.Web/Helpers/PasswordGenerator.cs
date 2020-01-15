namespace Unosquare.PassCore.Web.Helpers
{
    using SimpleBase;
    using System.Security.Cryptography;

    internal class PasswordGenerator : System.IDisposable
    {
        private readonly RNGCryptoServiceProvider _rngCsp = new RNGCryptoServiceProvider();

        public string Generate(int entropy)
        {
            var pswBytes = new byte[entropy];
            _rngCsp.GetBytes(pswBytes);

            var encoder = new Base85(Base85Alphabet.Ascii85);
            return encoder.Encode(pswBytes);
        }

        public void Dispose() => _rngCsp.Dispose();
    }
}
