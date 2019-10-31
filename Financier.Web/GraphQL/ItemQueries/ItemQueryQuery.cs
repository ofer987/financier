using System;
using System.Collections.Generic;
using GraphQL.Types;

using Financier.Common.Expenses.Models;

namespace Financier.Web.GraphQL.ItemQueries
{
    public class ItemQueryQuery : ObjectGraphType
    {
        public static class Keys
        {
            public static string TagNames = "tagNames";
            public static string FromMonth = "fromMonth";
            public static string FromYear = "fromYear";
            public static string ToMonth = "toMonth";
            public static string ToYear = "toYear";
        }

        public ItemQueryQuery()
        {
            Field<ListGraphType<MonthlyItemResultType>>(
                    "monthlyExpensesByTagNames",
                    arguments: new QueryArguments(
                        new QueryArgument<ListGraphType<NonNullGraphType<StringGraphType>>>
                        {
                            Name = Keys.TagNames,
                        },
                        new QueryArgument<NonNullGraphType<IntGraphType>>
                        {
                            Name = Keys.FromMonth
                        },
                        new QueryArgument<NonNullGraphType<IntGraphType>>
                        {
                            Name = Keys.FromYear
                        },
                        new QueryArgument<NonNullGraphType<IntGraphType>>
                        {
                            Name = Keys.ToMonth
                        },
                        new QueryArgument<NonNullGraphType<IntGraphType>>
                        {
                            Name = Keys.ToYear
                        }),
            resolve: context =>
             {
                 var tagNames = context.GetArgument<List<string>>(Keys.TagNames);
                 var fromMonth = context.GetArgument<int>(Keys.FromMonth);
                 var fromYear = context.GetArgument<int>(Keys.FromYear);
                 var toMonth = context.GetArgument<int>(Keys.ToMonth);
                 var toYear = context.GetArgument<int>(Keys.ToYear);

                 return new ItemQuery(
                         tagNames,
                         new DateTime(fromYear, fromMonth, 1),
                         new DateTime(toYear, toMonth, 1),
                         false
                 ).GetResultsOrderedByMonth();
             }
            );
        }
    }
}
