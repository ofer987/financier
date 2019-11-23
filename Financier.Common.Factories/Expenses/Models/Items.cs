using System;
using System.Collections.Generic;
using System.Linq;

using Financier.Common.Expenses.Models;

namespace Financier.Common
{
    public partial class Factories
    {
        public static Item ItemWithoutTags(Statement statement)
        {
            return new Item(
                id: Guid.NewGuid(),
                statementId: statement.Id,
                itemId: Guid.NewGuid().ToString(),
                description: "Item-Without-Tags",
                at: new DateTime(2019, 1, 2),
                amount: 10.00M
            )
            {
                ItemTags = new List<ItemTag>()
            };
        }

        public static Item ItemWithTags(Statement statement, IEnumerable<Tag> tags)
        {
            var itemId = Guid.NewGuid();

            return new Item(
                id: itemId,
                statementId: statement.Id,
                itemId: Guid.NewGuid().ToString(),
                description: "Item that has tags",
                at: new DateTime(2019, 1, 3),
                amount: 20.00M
            )
            {
                ItemTags = tags
                    .Select(tag => new ItemTag { ItemId = itemId, TagId = tag.Id })
                    .ToList()
            };
        }
    }
}
