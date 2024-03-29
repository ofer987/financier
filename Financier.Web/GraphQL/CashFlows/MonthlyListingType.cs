using GraphQL.Types;

using Financier.Common.Expenses;

namespace Financier.Web.GraphQL.CashFlows
{
    public class MonthlyListingType : ObjectGraphType<IMonthlyListing>
    {
        public MonthlyListingType()
        {
            Field(t => t.IsPrediction, nullable: false, type: typeof(BooleanGraphType));
            Field(t => t.Year, nullable: false, type: typeof(IntGraphType));
            Field(t => t.Month, nullable: false, type: typeof(IntGraphType));
            Field(t => t.Credit, nullable: false, type: typeof(DecimalGraphType));
            Field(t => t.Debit, nullable: false, type: typeof(DecimalGraphType));
            Field(t => t.Profit, nullable: false, type: typeof(DecimalGraphType));
        }
    }
}
