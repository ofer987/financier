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
            Field(t => t.AssetListings, nullable: false, type: typeof(ListGraphType<TagCostType>));
            Field(t => t.ExpenseListings, nullable: false, type: typeof(ListGraphType<TagCostType>));
            Field(t => t.AssetAmountTotal, nullable: false, type: typeof(DecimalGraphType));
            Field(t => t.ExpenseAmountTotal, nullable: false, type: typeof(DecimalGraphType));
        }
    }
}
