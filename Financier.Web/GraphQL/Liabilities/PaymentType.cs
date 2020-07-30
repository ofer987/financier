using GraphQL.DataLoader;
using GraphQL.Types;

using Financier.Common.Liabilities;

namespace Financier.Web.GraphQL.Liabilities
{
    public class PaymentType : ObjectGraphType<Payment>
    {
        public PaymentType(IDataLoaderContextAccessor _accessor)
        {
            Field("amount", t => t.Amount.Value, nullable: false, type: typeof(DecimalGraphType));
            Field("interest", t => t.Interest.Value, nullable: false, type: typeof(DecimalGraphType));
            Field("principal", t => t.Principal.Value, nullable: false, type: typeof(DecimalGraphType));
            Field("balance", t => t.Balance.Value, nullable: false, type: typeof(DecimalGraphType));
            Field(t => t.At, nullable: false, type: typeof(DateGraphType));
        }
    }
}
