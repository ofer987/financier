using System;
using System.Collections.Generic;
using System.Linq;

using Financier.Common.Expenses.Models;

namespace Financier.Tests.Expenses
{
    public partial class Fixtures
    {
        public static Item ItemWithoutTags(Statement statement)
        {
            return new Item
            {
                Id = Guid.NewGuid(),
                ItemId = Guid.NewGuid().ToString(),
                StatementId = statement.Id,
                Amount = 10.00M,
                Description = "Item-Without-Tags",
                TransactedAt = new DateTime(2019, 1, 2),
                ItemTags = new List<ItemTag>()
            };
        }

        public static Item ItemWithTags(Statement statement, IEnumerable<Tag> tags)
        {
            var itemId = Guid.NewGuid();

            return new Item
            {
                Id = itemId,
                ItemId = Guid.NewGuid().ToString(),
                StatementId = statement.Id,
                Amount = 20.00M,
                Description = "Item that has tags",
                TransactedAt = new DateTime(2019, 1, 3),
                ItemTags = tags
                    .Select(tag => new ItemTag { ItemId = itemId, TagId = tag.Id })
                    .ToList()
            };
        }
    }
}
