using System;
using System.Collections.Generic;
using System.Linq;
using GraphQL;
using GraphQL.Types;

using Financier.Common.Expenses;

namespace Financier.Web.GraphQL.CashFlows
{
    public class ProjectedCashFlowQuery : ObjectGraphType
    {
        public static class Keys
        {
            public static string Year = "year";
            public static string Month = "month";

            public static string FromYear = "fromYear";
            public static string FromMonth = "fromMonth";
            public static string ToYear = "toYear";
            public static string ToMonth = "toMonth";

            public static string ProjectedToYear = "projectedToYear";
            public static string ProjectedToMonth = "projectedToMonth";
        }

        public ProjectedCashFlowQuery()
        {
            Field<ListGraphType<ProjectedCashFlowType>>(
                "getMonthlyProjectedCashFlows",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<IntGraphType>>
                    {
                        Name = Keys.FromYear
                    },
                    new QueryArgument<NonNullGraphType<IntGraphType>>
                    {
                        Name = Keys.FromMonth
                    },
                    new QueryArgument<NonNullGraphType<IntGraphType>>
                    {
                        Name = Keys.ToYear
                    },
                    new QueryArgument<NonNullGraphType<IntGraphType>>
                    {
                        Name = Keys.ToMonth
                    },
                    new QueryArgument<NonNullGraphType<IntGraphType>>
                    {
                        Name = Keys.ProjectedToYear
                    },
                    new QueryArgument<NonNullGraphType<IntGraphType>>
                    {
                        Name = Keys.ProjectedToMonth
                    }
                ),
                resolve: context =>
                {
                    var fromYear = context.GetArgument<int>(Keys.FromYear);
                    var fromMonth = context.GetArgument<int>(Keys.FromMonth);
                    var toYear = context.GetArgument<int>(Keys.ToYear);
                    var toMonth = context.GetArgument<int>(Keys.ToMonth);

                    var projectedToYear = context.GetArgument<int>(Keys.ProjectedToYear);
                    var projectedToMonth = context.GetArgument<int>(Keys.ProjectedToMonth);

                    var fromDate = new DateTime(fromYear, fromMonth, 1);
                    var toDate = new DateTime(toYear, toMonth, 1);
                    var finalProjectedDate = new DateTime(projectedToYear, projectedToMonth, 1);
                    var projectedDates = GetProjectedDates(toDate, finalProjectedDate).ToList();

                    // Should this be converted to an array?
                    var existingValues = GetMonthlyAnalysis(fromDate, toDate).ToList();
                    // var projections = new ProjectedCashFlow(fromDate, toDate);
                    // var projectedValues = GetMonthlyProjections(projectedDates).ToList();

                    return projectedDates
                        .Select(date => this.GetMonthlyProjection(projections, date));
                }
            );
        }

        private IEnumerable<DurationCashFlow> GetMonthlyAnalysis(int year)
        {
            for (var month = 1; month <= 12; month += 1)
            {
                yield return new MonthlyCashFlow(year, month);
            }
        }

        private IEnumerable<DurationCashFlow> GetMonthlyAnalysis(DateTime fromDate, DateTime toDate)
        {
            var date = new DateTime(fromDate.Year, fromDate.Month, 1);
            var endDate = new DateTime(toDate.Year, toDate.Month, 1);

            for (; date <= endDate; date = date.AddMonths(1))
            {
                System.Console.WriteLine($"{date.Year}.{date.Month}");
                yield return new MonthlyCashFlow(date.Year, date.Month);
            }
        }

        private IEnumerable<DateTime> GetProjectedDates(DateTime fromDate, DateTime toDate)
        {
            for (var date = fromDate; date <= toDate; date = date.AddMonths(1))
            {
                yield return date;
            }
        }

        private MonthlyListing GetMonthlyProjection(ProjectedCashFlow cashFlowProjection, DateTime at)
        {
            // for (var date = cashFlowProjection.EndAt; date <= at; date = date.AddMonths(1))
            // {
            return cashFlowProjection.GetProjectedMonthlyListing(at);
            // }

            // return Enumerable.Empty<MonthlyListing>();
        }
    }
}
