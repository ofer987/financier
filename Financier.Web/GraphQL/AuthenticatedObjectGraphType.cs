using GraphQL;
using GraphQL.Types;

namespace Financier.Web.GraphQL
{
    public abstract class AuthenticatedObjectGraphType : ObjectGraphType
    {
        protected string GetEmail(IResolveFieldContext<object?> context)
        {
            var userContext = (context.UserContext as UserContext) ?? throw new UnauthorizedAccessException("User is not authenticated");

            if (!userContext.IsAuthenticated)
            {
                throw new UnauthorizedAccessException("User is not authenticated");
            }

            return userContext.Email;
        }
    }
}
