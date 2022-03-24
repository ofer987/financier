using System.Security.Claims;

namespace Financier.Web.Auth;

public class GraphQLUserContext : Dictionary<string, object>
{
    // TODO replace with NullablePrincipal
    public ClaimsPrincipal? User { get; init; }
}
