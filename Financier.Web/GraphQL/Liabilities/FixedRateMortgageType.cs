using GraphQL.DataLoader;
using GraphQL.Types;

using Financier.Common.Liabilities;

namespace Financier.Web.GraphQL.Liabilities
{
    public class FixedRateMortgageType : MortgageType<FixedRateMortgage>
    {
        public FixedRateMortgageType(IDataLoaderContextAccessor _accessor) : base(_accessor)
        {
        }
    }
}
