using System.Security.Principal;

namespace Financier.Web.Auth.GraphQL;

public class NullIdentity : IIdentity
{
    public string? AuthenticationType => string.Empty;
    public bool IsAuthenticated => false;
    public string? Name => string.Empty;
}
