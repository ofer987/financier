using System;
using System.Collections.Generic;
using System.Linq;

using Financier.Common.Expenses.Models;

namespace Financier.Common
{
    public partial class Factories
    {
        public static Item CreateItemWithoutTags(Statement statement, string itemId)
        {
            return new Item(
                id: Guid.NewGuid(),
                statementId: statement.Id,
                itemId: itemId,
                description: "Item-Without-Tags",
                at: new DateTime(2019, 1, 2),
                amount: 10.00M
            );
        }

        public static Item CreateItemWithTags(Statement statement, string itemId, IEnumerable<Tag> tags)
        {
            return CreateItemWithTags(statement, itemId, tags.Select(tag => tag.Name));
        }

        public static Item CreateItemWithTags(Statement statement, string itemId, IEnumerable<string> tagNames)
        {
            var result = CreateItemWithoutTags(statement, itemId);

            using (var db = new Context())
            {
               result.ItemTags = db.Tags
                    .Where(tag => tagNames.Any(searchedTagName => tag.Name == searchedTagName))
                    .AsEnumerable()
                    .Select(tag => new ItemTag { ItemId = result.Id, TagId = tag.Id })
                    .ToList();
            }

            return result;
        }
    }
}
