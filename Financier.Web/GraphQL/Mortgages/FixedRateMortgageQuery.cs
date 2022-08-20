using GraphQL;
using GraphQL.Types;

using Financier.Common.Liabilities;

namespace Financier.Web.GraphQL.Mortgages
{
    public class FixedRateMortgageQuery : AuthenticatedObjectGraphType
    {
        public static class Keys
        {
            public static string BaseValue = "baseValue";
            public static string InterestRate = "interestRate";
            public static string AmortisationPeriodInMonths = "amortisationPeriodInMonths";
            public static string InitiatedAt = "initiatedAt";
        }

        public FixedRateMortgageQuery()
        {
            Field<FixedRateMortgageGraphType>(
                "create",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<DecimalGraphType>>
                    {
                        Name = Keys.BaseValue
                    },
                    new QueryArgument<NonNullGraphType<DecimalGraphType>>
                    {
                        Name = Keys.InterestRate
                    },
                    new QueryArgument<IntGraphType>
                    {
                        Name = Keys.AmortisationPeriodInMonths,
                        DefaultValue = 300
                    },
                    new QueryArgument<DateGraphType>
                    {
                        Name = Keys.InitiatedAt,
                        DefaultValue = DateTime.Now
                    }
                ),
                resolve: context =>
                {
                    var baseValue = context.GetArgument<decimal>(Keys.BaseValue);
                    var interestRate = context.GetArgument<decimal>(Keys.InterestRate);
                    var amortisationPeriodInMonths = context.GetArgument<int>(Keys.AmortisationPeriodInMonths);
                    var initiatedAt = context.GetArgument<DateTime>(Keys.InitiatedAt);

                    return new FixedRateMortgage(baseValue, interestRate, amortisationPeriodInMonths, initiatedAt);
                }
            );
        }
    }
}
