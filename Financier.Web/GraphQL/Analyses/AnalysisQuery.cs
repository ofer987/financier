using System.Collections.Generic;
using System.Linq;
using GraphQL.Types;

using Financier.Common.Expenses;

namespace Financier.Web.GraphQL.Analyses
{
    public class AnalysisQuery : ObjectGraphType
    {
        public static class Keys
        {
            public static string Year = "year";
            public static string Month = "month";
        }

        public AnalysisQuery()
        {
            Field<AnalysisType>(
                "analysis",
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

                    return new MonthlyAnalysis(year, month);
                }
            );

            Field<ListGraphType<AnalysisType>>(
                "analyses",
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
        }

        private IEnumerable<Analysis> GetMonthlyAnalysis(int year)
        {
            for (var month = 1; month <= 12; month += 1)
            {
                yield return new MonthlyAnalysis(year, month);
            }
        }
    }
}
