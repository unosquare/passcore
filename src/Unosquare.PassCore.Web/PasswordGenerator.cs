namespace Unosquare.PassCore.Web
{
    using SimpleBase;
    using System.Security.Cryptography;

    public static class PasswordGenerator
    {
        public static string Generate(RNGCryptoServiceProvider provider, int entropy)
        {
            var pswBytes = new byte[entropy];
            provider.GetBytes(pswBytes);

            var enconder = new Base85(Base85Alphabet.Ascii85);
            return enconder.Encode(pswBytes);
        }
    }
}
