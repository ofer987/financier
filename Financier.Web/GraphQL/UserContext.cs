using System.Security.Principal;

namespace Financier.Web.GraphQL;

public class UserContext : Dictionary<string, object>
{
    public IPrincipal User { get; init; } = new NullPrincipal();
}
