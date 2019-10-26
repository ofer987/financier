using System;
using System.Collections.Generic;
using System.Linq;
using GraphQL.Types;

using Financier.Common.Expenses;
using Financier.Common.Expenses.Models;
using Financier.Web.GraphQL.MoreTypes;

namespace Financier.Web.GraphQL.Items
{
    public class ItemQuery : ObjectGraphType
    {
        public static class Keys
        {
            public static string TagNames = "tagNames";
            public static string FromMonth = "fromMonth";
            public static string FromYear = "fromYear";
            public static string ToMonth = "toMonth";
            public static string ToYear = "toYear";
        }

        public ItemQuery()
        {
            Field<ListGraphType<ItemType>>(
                "items",
                resolve: _ => Item.GetAll()
            );

            Field<ItemType>(
                "item",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<IdGraphType>>
                    {
                        Name = "id"
                    }
                ),
                resolve: context =>
                {
                    var id = context.GetArgument<Guid>("id");
                    return Item.Get(id);
                }
            );

            Field<ItemType>(
                "itemByItemId",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<IdGraphType>>
                    {
                        Name = "itemId"
                    }
                ),
                resolve: context =>
                {
                    var itemId = context.GetArgument<string>("itemId");
                    return Item.GetByItemId(itemId);
                }
            );

            Field<ListGraphType<ItemType>>(
                "itemsByTagId",
                arguments: new QueryArguments(
                    new QueryArgument<ListGraphType<NonNullGraphType<IdGraphType>>>
                    {
                        Name = "tagIds"
                    }
                ),
                resolve: context =>
                {
                    var tagIds = context.GetArgument<List<Guid>>("tagIds");
                    return Item.GetByTagIds(tagIds);
                }
            );

            Field<ListGraphType<ItemType>>(
                "expensesByTagNames",
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

                    return new Analysis(
                        new DateTime(fromYear, fromMonth, 1),
                        new DateTime(toYear, toMonth, 1)
                    ).GetItemsByTags(false, tagNames);
                }
            );

            Field<ListGraphType<MonthlyAmount>>(
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

                 return new Analysis(
                     new DateTime(fromYear, fromMonth, 1),
                     new DateTime(toYear, toMonth, 1)
                 ).GetItemsByTagsSortedByDate(false, tagNames).ToList();
             }
            );
        }
    }
}
