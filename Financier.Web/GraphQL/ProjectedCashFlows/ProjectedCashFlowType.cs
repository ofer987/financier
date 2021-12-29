using GraphQL.DataLoader;
using GraphQL.Types;

using Financier.Common.Expenses;

namespace Financier.Web.GraphQL.CashFlows
{
    public class ProjectedCashFlowType : ObjectGraphType<ProjectedCashFlow>
    {
        public ProjectedCashFlowType(IDataLoaderContextAccessor dataLoaderAccessor)
        {
            Field(t => t.StartAt, nullable: false, type: typeof(DateTimeGraphType));
            Field(t => t.EndAt, nullable: false, type: typeof(DateTimeGraphType));
            Field(t => t.CreditAmountTotal, nullable: false, type: typeof(ListGraphType<DecimalGraphType>));
            Field(t => t.DebitAmountTotal, nullable: false, type: typeof(ListGraphType<DecimalGraphType>));
        }
    }
}
