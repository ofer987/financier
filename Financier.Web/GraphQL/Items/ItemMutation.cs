using System;
using System.Collections.Generic;
using GraphQL;
using GraphQL.Types;

using Financier.Common.Expenses;
using Financier.Common.Expenses.Models;

namespace Financier.Web.GraphQL.Items
{
    public class ItemMutation : ObjectGraphType
    {
        private class Arguments
        {
            public static string NewTags = "newTags";

            public class Item
            {
                public static string Id = "itemId";
            }
        }

        public ItemMutation()
        {
            Field<NonNullGraphType<BooleanGraphType>>(
                "updateTags",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<ListGraphType<StringGraphType>>> { Name = Arguments.NewTags },
                    new QueryArgument<NonNullGraphType<IdGraphType>> { Name = Arguments.Item.Id }),
                resolve: (context) =>
                {
                    var itemId = context.GetArgument<Guid>(Arguments.Item.Id);
                    var newTags = context.GetArgument<IEnumerable<string>>(Arguments.NewTags);

                    var item = Item.Get(itemId);
                    item.UpdateTags(newTags);

                    // TODO: replace BooleanGraphType with a GraphType that
                    // returns null
                    return true;
                }
            );
        }
    }
}
