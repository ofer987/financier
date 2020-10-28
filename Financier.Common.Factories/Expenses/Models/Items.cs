using System;
using System.Collections.Generic;
using System.Linq;

using Financier.Common.Expenses.Models;

namespace Financier.Common
{
    public partial class Factories
    {
        public static Item CreateItemWithoutTags(Statement statement, string itemId, string description, DateTime at, decimal amount)
        {
            return new Item(
                id: Guid.NewGuid(),
                statementId: statement.Id,
                itemId: itemId,
                description: description,
                at: at,
                amount: amount
            );
        }

        public static Item CreateItemWithTags(Statement statement, string itemId, string description, DateTime at, decimal amount, IEnumerable<Tag> tags)
        {
            return CreateItemWithTags(
                statement,
                itemId,
                description,
                at,
                amount,
                tags.Select(tag => tag.Name)
            );
        }

        public static Item CreateItemWithTags(Statement statement, string itemId, string description, DateTime at, decimal amount, IEnumerable<string> tagNames)
        {
            var result = CreateItemWithoutTags(
                statement,
                itemId,
                description,
                at,
                amount
            );

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
