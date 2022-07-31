using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;

namespace Financier.Web.Extensions;

public static class JwtExtensions
{
    public static bool IsTokenValid(this JwtSecurityToken self, string publicKey)
    {
        var tokenParts = self.RawCiphertext.Split('.');

        var rsa = new RSACryptoServiceProvider();
        rsa.ImportParameters(
                new RSAParameters() {
                Modulus = FromBase64Url(publicKey),
                Exponent = FromBase64Url("AQAB"),
                }
                );

        var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(tokenParts[0] + '.' + tokenParts[1]));

        var rsaDeformatter = new RSAPKCS1SignatureDeformatter(rsa);
        rsaDeformatter.SetHashAlgorithm("SHA256");

        return rsaDeformatter.VerifySignature(hash, FromBase64Url(tokenParts[2]));
    }

    private static byte[] FromBase64Url(string base64Url)
    {
        string padded = base64Url.Length % 4 == 0
            ? base64Url : base64Url + "====".Substring(base64Url.Length % 4);
        string base64 = padded.Replace("_", "/")
            .Replace("-", "+");
        return Convert.FromBase64String(base64);
    }
}
