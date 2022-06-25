using GraphQL;
using GraphQL.Instrumentation;

namespace Financier.Web.GraphQL.CashFlows
{
    public class UserContextMiddleware : IFieldMiddleware
    {
        public async ValueTask<object?> ResolveAsync(IResolveFieldContext context, FieldMiddlewareDelegate next)
        {
            if (!IsAuthenticated(context.UserContext))
            {
                throw new UnauthorizedAccessException("User is not authenticated");
            }

            return await next(context);
        }

        private bool IsAuthenticated(IDictionary<string, object?> userContext)
        {
            return ((userContext is UserContext) && (userContext as UserContext)!.IsAuthenticated);
        }
    }
}
