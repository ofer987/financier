using GraphQL.Types;
using GraphQL.Instrumentation;

namespace Financier.Web.GraphQL.Mortgages
{
    public class FixedRateMortgageSchema : Schema
    {
        public FixedRateMortgageSchema(IServiceProvider provider, IEnumerable<IFieldMiddleware> middlewares) : base(provider)
        {
            Query = new FixedRateMortgageQuery();

            foreach (var middleware in middlewares)
            {
                FieldMiddleware.Use(middleware);
            }
        }
    }
}
