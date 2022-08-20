using System.Security.Principal;

namespace Financier.Web.GraphQL;

public class NullPrincipal : IPrincipal
{
    public IIdentity? Identity => new NullIdentity();

    public bool IsInRole(string role)
    {
        return false;
    }
}
