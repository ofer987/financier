using System;
using System.Collections.Generic;
using GraphQL;
using GraphQL.Types;

using Financier.Common.Expenses.Models;

namespace Financier.Web.GraphQL.Items
{
    public class ItemQuery : ObjectGraphType
    {
        public static class Keys
        {
            public static string Id = "id";
            public static string ItemId = "itemId";
            public static string TagNames = "tagNames";
            public static string TagIds = "tagIds";
            public static string PostedAt = "postedAt";
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
                        Name = Keys.Id
                    }
                ),
                resolve: context =>
                {
                    var id = context.GetArgument<Guid>(Keys.Id);
                    return Item.Get(id);
                }
            );

            Field<ItemType>(
                "itemByItemId",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<IdGraphType>>
                    {
                        Name = Keys.ItemId
                    }
                ),
                resolve: context =>
                {
                    var itemId = context.GetArgument<string>(Keys.ItemId);
                    return Item.GetByItemId(itemId);
                }
            );

            Field<ListGraphType<ItemType>>(
                "itemsByTagId",
                arguments: new QueryArguments(
                    new QueryArgument<ListGraphType<NonNullGraphType<IdGraphType>>>
                    {
                        Name = Keys.TagIds
                    }
                ),
                resolve: context =>
                {
                    var tagIds = context.GetArgument<List<Guid>>(Keys.TagIds);
                    return Item.GetByTagIds(tagIds);
                }
            );

            Field<ListGraphType<ItemType>>(
                "itemsByTagNamesAndPostedAt",
                arguments: new QueryArguments(
                    new QueryArgument<ListGraphType<NonNullGraphType<StringGraphType>>>
                    {
                        Name = Keys.TagNames
                    },
                    new QueryArgument<NonNullGraphType<DateGraphType>>
                    {
                        Name = Keys.PostedAt
                    }
                ),
                resolve: context =>
                {
                    var tagNames = context.GetArgument<List<string>>(Keys.TagNames);
                    var postedAt = context.GetArgument<DateTime>(Keys.PostedAt);
                    var fromDate = new DateTime(postedAt.Year, postedAt.Month, 1);
                    var toDate = fromDate.AddMonths(1);

                    return Item.GetAllBy(fromDate, toDate, tagNames);
                }
            );
        }
    }
}
