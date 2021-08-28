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
        }
    }
}
