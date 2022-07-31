using System.Text.RegularExpressions;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

using Financier.Common.Extensions;
using Financier.Web.Extensions;

namespace Financier.Web.GraphQL;

public class UserContext : Dictionary<string, object?>
{
    const string PublicKey = "nN6M3RHeDNE57lTiYTtXNnpADuEu1EmbBLPWzkTVAMLlxL5q-kbhUfMSW4rUDwQNaTmJtBfgqhTy-yUvT_DEs2v3ebW_Pw6csNTlaY6zvjPP4qvgRhC7xsPfU9pRWi8br_GBs7OYnWjjXa-Pfx9oRDOdRnSxqI9t44rbqB1E5n-VoeICQY2hHqmtWL2rOb-qdyYuWazTsDr7aXc_inBSf6SCA6ASWjQRy6Ijk1TWvUk1-yBz4gmp72AvhHRNMsY1VHxF5FsQrf2PhlrLZpOUR5Oy__24GAQhvB6Q1GJqnRCGwa0S12WuNJDjHBElYE6tluto5Q65bl525-a8ejDcpQ";

    public UserContext(string authenticationValue)
    {
        var jwtRegex = new Regex(@"^Bearer (.*)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        var rawJwt = jwtRegex.Match(authenticationValue).Groups[1].Value;
        var jwt = new JwtSecurityToken(rawJwt);

        if (!jwt.IsTokenValid(PublicKey))
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
