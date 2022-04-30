using System;
using System.Collections.Generic;
using System.Linq;

using Financier.Common.Expenses.Models;

namespace Financier.Common
{
    public partial class Factories
    {
        public static Item NewItem(Statement statement, string itemId, string description, DateTime at, decimal amount)
        {
            var utcAt = DateTime.SpecifyKind(at, DateTimeKind.Utc);

            return new Item
            {
                StatementId = statement.Id,
                ItemId = itemId,
                Amount = amount,
                Description = description,
                PostedAt = utcAt,
                TransactedAt = utcAt
            };
        }

        public static Item CreateItemWithoutTags(Statement statement, string itemId, string description, DateTime at, decimal amount)
        {
            var utcAt = DateTime.SpecifyKind(at, DateTimeKind.Utc);

            using (var db = new Context())
            {
                return new Item(
                    id: Guid.NewGuid(),
                    statementId: statement.Id,
                    itemId: itemId,
                    description: description,
                    at: utcAt,
                    amount: amount
                );
            }
        }

        public static Item CreateItemWithTags(Statement statement, string itemId, string description, DateTime at, decimal amount, IEnumerable<Tag> tags)
        {
            var utcAt = DateTime.SpecifyKind(at, DateTimeKind.Utc);

            Item item;
            using (var db = new Context())
            {
                item = CreateItemWithoutTags(
                    statement,
                    itemId,
                    description,
                    utcAt,
                    amount
                );
                db.Items.Add(item);
                db.SaveChanges();
            }

            using (var db = new Context())
            {
                var newItemTags = tags
                    .Select(tag => new ItemTag { ItemId = item.Id, TagId = tag.Id })
                    .ToList();

                db.ItemTags.AddRange(newItemTags);
                db.SaveChanges();
            }

            return item;
        }
    }
}
