using System;
using System.Collections.Generic;

using Financier.Common;
using Financier.Common.Models.Expenses;

namespace Financier.Tests.Fixtures
{
    public static class Items
    {
        public static Item ItemWithoutTags(Statement statement)
        {
            return new Item
            {
                Id = Guid.NewGuid(),
                StatementId = statement.Id,
                Amount = 10.00M,
                Description = "Item-Without-Tags",
                TransactedAt = new DateTime(2019, 1, 2),
                ItemTags = new List<ItemTag>()
            };
        }

        public static Item CreateItemWithRonAndDanTags(Statement statement)
        {
            var item = new Item
            {
                Id = Guid.NewGuid(),
                StatementId = statement.Id,
                Amount = 20.00M,
                Description = "Item that has Dan and Ron tags",
                TransactedAt = new DateTime(2019, 1, 3),
                ItemTags = new List<ItemTag>()
            };

            var tags = new [] { Tags.DanTag(), Tags.RonTag() };
            List<ItemTag> itemTags = new List<ItemTag>();

            foreach (var tag in tags)
            {
                var itemTag = new ItemTag { ItemId = item.Id, TagId = tag.Id };
                itemTags.Add(itemTag);
                item.ItemTags.Add(itemTag);
            }

            using (var db = new ExpensesContext())
            {
                db.Tags.AddRange(tags);
                db.SaveChanges();

                db.ItemTags.AddRange(itemTags);
                db.SaveChanges();

                db.Items.Add(item);
                db.SaveChanges();
            }

            return item;
        }
    }
}
