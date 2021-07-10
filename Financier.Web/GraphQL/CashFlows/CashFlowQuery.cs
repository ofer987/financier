using System.Collections.Generic;
using System.Linq;
using GraphQL;
using GraphQL.Types;

using Financier.Common.Expenses;

namespace Financier.Web.GraphQL.CashFlows
{
    public class CashFlowQuery : ObjectGraphType
    {
        public static class Keys
        {
            public static string Year = "year";
            public static string Month = "month";
        }

        public CashFlowQuery()
        {
            Field<CashFlowType>(
                "getMonthlyCashFlow",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<IntGraphType>>
                    {
                        Name = Keys.Year
                    },
                    new QueryArgument<NonNullGraphType<IntGraphType>>
                    {
                        Name = Keys.Month
                    }
                ),
                resolve: context =>
                {
                    var year = context.GetArgument<int>(Keys.Year);
                    var month = context.GetArgument<int>(Keys.Month);

                    return new MonthlyCashFlow(year, month);
                }
            );

            Field<ListGraphType<CashFlowType>>(
                "getMonthlyCashFlows",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<IntGraphType>>
                    {
                        Name = Keys.Year
                    }
                ),
                resolve: context =>
                {
                    var year = context.GetArgument<int>(Keys.Year);

                    // Should this be converted to an array?
                    return GetMonthlyAnalysis(year).ToList();
                }
            );

            Field<CashFlowType>(
                "getYearlyCashFlow",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<IntGraphType>>
                    {
                        Name = Keys.Year
                    }
                ),
                resolve: context =>
                {
                    var year = context.GetArgument<int>(Keys.Year);

                    return new YearlyCashFlow(year);
                }
            );
        }

        private IEnumerable<CashFlow> GetMonthlyAnalysis(int year)
        {
            for (var month = 1; month <= 12; month += 1)
            {
                yield return new MonthlyCashFlow(year, month);
            }
        }
    }
}
