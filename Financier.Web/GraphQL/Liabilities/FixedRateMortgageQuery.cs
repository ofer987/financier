using GraphQL.Types;

namespace Financier.Web.GraphQL.Liabilities
{
    public class FixedRateMortgageQuery : ObjectGraphType
    {
        public static class Keys
        {
            public static string Year = "year";
            public static string Month = "month";
            public static string Day = "day";
        }

        public FixedRateMortgageQuery()
        {
            Field<DecimalGraphType>(
                "balance",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<IntGraphType>>
                    {
                        Name = Keys.Year
                    },
                    new QueryArgument<NonNullGraphType<IntGraphType>>
                    {
                        Name = Keys.Month
                    },
                    new QueryArgument<NonNullGraphType<IntGraphType>>
                    {
                        Name = Keys.Day
                    }
                ),
                resolve: context =>
                {
                    // var year = context.GetArgument<int>(Keys.Year);
                    // var month = context.GetArgument<int>(Keys.Month);
                    // var day = context.GetArgument<int>(Keys.Day);
                    //
                    // var at = new DateTime(year, month, day);

                    return 0.00M;
                }
            );
        }
    }
}
