using System;
using System.Collections.Generic;
using GraphQL.Types;

using Financier.Common.Expenses.Models;

namespace Financier.Web.GraphQL
{
    public class ItemQuery : ObjectGraphType
    {
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
                "updateTags",
                arguments: new QueryArguments(
                    new QueryArgument<ListGraphType<NonNullGraphType<StringGraphType>>>
                    {
                        Name = "tagNames"
                    }
                ),
                resolve: context =>
                {
                    var tagNames = context.GetArgument<List<string>>("tagNames");
                    return Item.GetByTagNames(tagNames);
                }
            );
        }
    }
}
