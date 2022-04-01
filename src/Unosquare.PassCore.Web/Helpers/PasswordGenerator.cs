using SimpleBase;
using System.Security.Cryptography;

namespace Unosquare.PassCore.Web.Helpers;

internal class PasswordGenerator : System.IDisposable
{
    private readonly RNGCryptoServiceProvider _rngCsp = new();

    public string Generate(int entropy)
    {
        var pswBytes = new byte[entropy];
        _rngCsp.GetBytes(pswBytes);

        var encoder = new Base85(Base85Alphabet.Ascii85);
        return encoder.Encode(pswBytes);
    }

    public void Dispose() => _rngCsp.Dispose();
}