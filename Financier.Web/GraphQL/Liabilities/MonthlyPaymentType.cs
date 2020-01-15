using GraphQL.DataLoader;
using GraphQL.Types;

using Financier.Common.Liabilities;

namespace Financier.Web.GraphQL.Liabilities
{
    public class MonthlyPaymentType : ObjectGraphType<MonthlyPayment>
    {
        public MonthlyPaymentType(IDataLoaderContextAccessor _accessor)
        {
            Field(t => t.Amount, nullable: false, type: typeof(DecimalGraphType));
            Field(t => t.Interest, nullable: false, type: typeof(DecimalGraphType));
            Field(t => t.Principal, nullable: false, type: typeof(DecimalGraphType));
            Field(t => t.Balance, nullable: false, type: typeof(DecimalGraphType));
            Field(t => t.At, nullable: false, type: typeof(DateGraphType));
        }
    }
}
