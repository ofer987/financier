using GraphQL.DataLoader;
using GraphQL.Types;

using Financier.Common.Expenses;
using Financier.Web.GraphQL.CashFlows;
using Financier.Web.GraphQL.Liabilities;

namespace Financier.Web.GraphQL.Expenses
{
    public class MyHomeType : ObjectGraphType<MyHome>
    {
        public MyHomeType(IDataLoaderContextAccessor _accessor)
        {
            // FIXME: use a generic Mortgage type
            Field(t => t.Mortgage, nullable: false, type: typeof(FixedRateMortgageType));
            Field(t => t.CashFlow, nullable: false, type: typeof(CashFlowType));
            Field(t => t.InitialCash, nullable: false, type: typeof(DecimalGraphType));
            Field(t => t.InitialDebt, nullable: false, type: typeof(DecimalGraphType));
        }
    }
}
