using GraphQL.Types;

using Financier.Common.Liabilities;

namespace Financier.Web.GraphQL.Mortgages
{
    public class FixedRateMortgageGraphType : ObjectGraphType<FixedRateMortgage>
    {
        public FixedRateMortgageGraphType()
        {
            Field(t => t.Price, nullable: false, type: typeof(DecimalGraphType));
            Field(t => t.InitialValue, nullable: false, type: typeof(DecimalGraphType));
            Field(t => t.InitiatedAt, nullable: false, type: typeof(DateGraphType));
            Field(t => t.AmortisationPeriodInMonths, nullable: false, type: typeof(IntGraphType));
            Field(t => t.InterestRate, nullable: false, type: typeof(DecimalGraphType));
            Field(t => t.PeriodicMonthlyInterestRate, nullable: false, type: typeof(FloatGraphType));
            Field("MinimumMonthlyPayment", t => t.MonthlyPayment, nullable: false, type: typeof(FloatGraphType));
        }
    }
}
