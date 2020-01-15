using GraphQL;
using GraphQL.Types;

namespace Financier.Web.GraphQL.Liabilities
{
    public class FixedRateMortgageSchema : Schema
    {
        public FixedRateMortgageSchema(IDependencyResolver resolver) : base(resolver)
        {
            Query = resolver.Resolve<FixedRateMortgageQuery>();
        }
    }
}
