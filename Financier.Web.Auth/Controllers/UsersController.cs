using System;
using Microsoft.AspNetCore.Mvc;
using JWT.Builder;
using JWT.Algorithms;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

using Financier.Web.Auth.Models;

namespace Financier.Web.Auth.Controllers;

public class UsersController : Controller
{
    private const string PRIVATE_KEY_PATH = "financier_rsa.pem";
    private const string PUBLIC_KEY_PATH = "financier_rsa.pub.pem";

    public string Index()
    {
        return "dan@ofer.to";
    }

    [HttpPost]
    public string Create([FromBody] User user)
    {
        Console.WriteLine(user.Name);
        return CreateJwtToken(user.Name);
    }

    private string CreateJwtToken(string name)
    {
        // var certifcationPath = "root.cert";
        // var certifcationPath = "root.cert";
        // var certificate = new X509Certificate(certifcationPath);

        var privateKey = ReadKey(PRIVATE_KEY_PATH);
        var publicKey = ReadKey(PUBLIC_KEY_PATH);
        var rsa = new RS256Algorithm(publicKey, privateKey);
        // var certifcationPath = "root.cert";
        // var certificate = new X509Certificate(certifcationPath);
        // var rsa = new RS256Algorithm(new X509Certificate2(certificate));

        var result = JwtBuilder.Create()
            .WithAlgorithm(rsa)
            // TODO: change to true?
            .WithVerifySignature(false)
            .AddHeader("kid", "1549e0aef574d1c7bdd136c202b8d290580b165c")
            .AddClaim("exp", DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds())
            .AddClaim("email", name)
            .Encode();

        return result;
    }

    private RSA ReadKey(string path)
    {
        var result = RSA.Create();
        var text = System.IO.File.ReadAllText(path);

        result.ImportFromPem(text);

        return result;
    }

    // private RSA GetPublicKey()
    // {
    //     var result = RSA.Create();
    //     result.ImportRSAPublicKey(Convert.FromBase64String(File.ReadAllText(PUBLIC_KEY_PATH)), out var _bytesRead);
    //
    //     return result;
    // }
}
