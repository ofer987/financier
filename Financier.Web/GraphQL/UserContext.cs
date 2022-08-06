using System.Text.RegularExpressions;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

using Financier.Common.Extensions;
using Financier.Web.Extensions;

namespace Financier.Web.GraphQL;

public class UserContext : Dictionary<string, object?>
{
    const string PublicKey = "stD2wMn0tZrAn8FnW9LdQozV9qNatcdbJ6R7V6ag5XNzJdFfdD5vOOGw5n8SJ-vqg69rK322kha5vkLODd6hdtn5R0KMiOVAD8Gf8DmKfSafSBZm7ImacVawagdvBcVGfiLqQEXavRLcDDLsTOAI9sIyH6KU2Rmg9RUdFbgTyZj_9mp4vzHIBG8ED71oqHv41KX4v3Ku4kE7x93a34QOdqkKXn6GZUCbC9urWB4UNX7rg_Bds2pfwutC3QhWS109GnPO7Mvt_XaUOyKavwiMNH4Bv7nD9rGflffh7WYmzosQPZvyY5b9Flhiev180DPfQ7J5vca5MO263AyWBv4roQ";

    public UserContext(string authenticationValue)
    {
        var jwtRegex = new Regex(@"^Bearer (.*)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        var rawJwt = jwtRegex.Match(authenticationValue).Groups[1].Value;
        var jwt = new JwtSecurityToken(rawJwt);

        if (!jwt.IsTokenValid(rawJwt, PublicKey))
        {
            throw new UnauthorizedAccessException("Token is not valid");
        }

        if (jwt.ValidTo != DateTime.MinValue && jwt.ValidTo < DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException("Token is expired");
        }

        var identity = new ClaimsIdentity(jwt.Claims);
        User = new ClaimsPrincipal(new List<ClaimsIdentity> { identity });
    }

    public ClaimsPrincipal User { get; private set; }

    public string Email => User.Claims.First(item => item.Type == "email").Value;

    public bool IsAuthenticated => !Email.IsNullOrWhiteSpace();
}
