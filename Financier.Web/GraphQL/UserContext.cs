using System.Text.RegularExpressions;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

using Financier.Common.Extensions;
namespace Financier.Web.GraphQL;

public class UserContext : Dictionary<string, object?>
{
    public UserContext(string authenticationValue)
    {
        var jwtRegex = new Regex(@"^Bearer (.*)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        var rawJwt = jwtRegex.Match(authenticationValue).Groups[1].Value;
        var jwt = new JwtSecurityToken(rawJwt);

        var identity = new ClaimsIdentity(jwt.Claims);
        User = new ClaimsPrincipal(new List<ClaimsIdentity> { identity });
    }

    public ClaimsPrincipal User { get; private set; }

    public string Email => User.Claims.First(item => item.Type == "email").Value;

    public bool IsAuthenticated => !Email.IsNullOrWhiteSpace();
}
