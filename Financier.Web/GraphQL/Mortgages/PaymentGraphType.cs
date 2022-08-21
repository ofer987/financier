using GraphQL.Types;

using Financier.Common.Liabilities;

namespace Financier.Web.GraphQL.Mortgages
{
    public class PaymentGraphType : ObjectGraphType<Payment>
    {
        public PaymentGraphType()
        {
            Field(t => t.Amount, nullable: false, type: typeof(DecimalGraphType));
            Field(t => t.Interest, nullable: false, type: typeof(DecimalGraphType));
            Field(t => t.Principal, nullable: false, type: typeof(DateGraphType));
            Field(t => t.Balance, nullable: false, type: typeof(IntGraphType));
            Field(t => t.At, nullable: false, type: typeof(DateGraphType));
        }
    }
}
