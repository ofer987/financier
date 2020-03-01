using GraphQL.DataLoader;
using GraphQL.Types;

using Financier.Common.Expenses;
using Financier.Web.GraphQL.CashFlows;

namespace Financier.Web.GraphQL.Expenses
{
    public class BalanceSheetType : ObjectGraphType<BalanceSheet>
    {
        public BalanceSheetType(IDataLoaderContextAccessor _accessor)
        {
            // FIXME: use a generic Mortgage type
            // Field(t => t.Home, nullable: false, type: typeof(Home));
            // Field(t => t.CashFlow, nullable: false, type: typeof(CashFlowType));
            // Field(t => t.InitialCash.Value, nullable: false, type: typeof(DecimalGraphType));
            // Field(t => t.InitialDebt.Value, nullable: false, type: typeof(DecimalGraphType));
        }
    }
}
