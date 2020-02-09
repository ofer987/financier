using GraphQL.DataLoader;
using GraphQL.Types;

using Financier.Common.Expenses;
using Financier.Web.GraphQL.TagCosts;

namespace Financier.Web.GraphQL.CashFlows
{
    public class CashFlowType : ObjectGraphType<CashFlow>
    {
        public CashFlowType(IDataLoaderContextAccessor dataLoaderAccessor)
        {
            Field(t => t.StartAt, nullable: false, type: typeof(DateTimeGraphType));
            Field(t => t.EndAt, nullable: false, type: typeof(DateTimeGraphType));
            Field(t => t.CreditListings, nullable: false, type: typeof(ListGraphType<TagCostType>));
            Field(t => t.DebitListings, nullable: false, type: typeof(ListGraphType<TagCostType>));
            Field(t => t.CreditAmountTotal, nullable: false, type: typeof(DecimalGraphType));
            Field(t => t.DebitAmountTotal, nullable: false, type: typeof(DecimalGraphType));
        }
    }
}
