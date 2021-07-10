using System;
using GraphQL.Utilities;
using GraphQL.Types;

namespace Financier.Web.GraphQL.Liabilities
{
    public class FixedRateMortgageSchema : Schema
    {
        public FixedRateMortgageSchema(IServiceProvider provider) : base(provider)
        {
            Query = provider.GetRequiredService<FixedRateMortgageQuery>();
        }
    }
}
