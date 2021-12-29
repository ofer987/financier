using GraphQL.DataLoader;
using GraphQL.Types;

using Financier.Common.Expenses;

namespace Financier.Web.GraphQL.CashFlows
{
    public class MonthlyListingType : ObjectGraphType<MonthlyListing>
    {
        public MonthlyListingType(IDataLoaderContextAccessor dataLoaderAccessor)
        {
            Field(t => t.Year, nullable: false, type: typeof(IntGraphType));
            Field(t => t.Month, nullable: false, type: typeof(IntGraphType));
            Field(t => t.CreditAmount, nullable: false, type: typeof(ListGraphType<DecimalGraphType>));
            Field(t => t.DebitAmount, nullable: false, type: typeof(ListGraphType<DecimalGraphType>));
            Field(t => t.Profit, nullable: false, type: typeof(ListGraphType<DecimalGraphType>));
        }
    }
}
