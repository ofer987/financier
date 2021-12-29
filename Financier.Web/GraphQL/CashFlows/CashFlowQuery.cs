using System;
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

            public static string FromYear = "fromYear";
            public static string FromMonth = "fromMonth";
            public static string ToYear = "toYear";
            public static string ToMonth = "toMonth";

            public static string ProjectedToYear = "projectedToYear";
            public static string ProjectedToMonth = "projectedToMonth";
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
                "getMonthlyCashFlowsByYear",
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

            Field<ListGraphType<CashFlowType>>(
                "getMonthlyCashFlows",
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
                    }
                ),
                resolve: context =>
                {
                    var fromYear = context.GetArgument<int>(Keys.FromYear);
                    var fromMonth = context.GetArgument<int>(Keys.FromMonth);
                    var toYear = context.GetArgument<int>(Keys.ToYear);
                    var toMonth = context.GetArgument<int>(Keys.ToMonth);

                    var fromDate = new DateTime(fromYear, fromMonth, 1);
                    var toDate = new DateTime(toYear, toMonth, 1);

                    // Should this be converted to an array?
                    return GetMonthlyAnalysis(fromDate, toDate).ToList();
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

            Field<ListGraphType<MonthlyListingType>>(
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
                    // var cashFlow = new ProjectedCashFlow(
                    // var projectedDates = GetProjectedDates(toDate, finalProjectedDate).ToList();

                    var cashflows = new ProjectedCashFlow(fromDate, toDate);

                    // Should this be converted to an array?
                    var existingListings = this.GetExistingDates(fromDate, toDate)
                        .Select(date => cashflows.GetMonthlyListing(date.Year, date.Month));

                    var projectedListings = this.GetProjectedDates(toDate, finalProjectedDate)
                        .Select(date => cashflows.GetProjectedMonthlyListing(date.Year, date.Month));

                    // var existingMonthlyCashFlows = cashflows.GetMonthlyListing( GetMonthlyCashFlows(fromDate, toDate);
                    // var predictedMonthlyCashFlows = GetMonthlyProjections(fromDate, toDate, finalProjectedDate);

                    return existingListings
                        .Concat(projectedListings)
                        .ToList();

                    // return existingMonthlyCashFlows.Concat(predictedMonthlyCashFlows).ToList();
                    // var projectedValues = GetMonthlyProjections(projectedDates).ToList();

                    // return projectedDates
                    //     .Select(date => this.GetMonthlyProjection(projections, date));
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

        private IEnumerable<DateTime> GetExistingDates(DateTime startAt, DateTime endAt)
        {
            for (var date = startAt; date <= endAt; date = date.AddMonths(1))
            {
                yield return date;
            }
        }

        private IEnumerable<DateTime> GetProjectedDates(DateTime endAt, DateTime projectedFinalAt)
        {
            for (var date = endAt; date <= projectedFinalAt; date = date.AddMonths(1))
            {
                yield return date;
            }
        }
    }
}
