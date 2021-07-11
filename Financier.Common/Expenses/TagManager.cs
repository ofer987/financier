using System;
using System.Collections.Generic;
using System.Linq;

using Financier.Common.Extensions;
using Financier.Common.Expenses.Models;

namespace Financier.Common.Expenses
{
    // TODO: Remove this class and its tests!
    public class TagManager
    {
        public Item Item { get; set; }

        public static List<Tag> FindOrCreateTags(string list)
        {
            var tags = new List<Tag>();

            var names = list
                .Split(',')
                .Select(item => item.Trim().ToLower())
                .Reject(item => item.IsNullOrEmpty())
                .Distinct();

            using (var db = new Context())
            {
                foreach (var name in names)
                {
                    var tag = db.Tags
                        .FirstOrDefault(t => t.Name == name);

                    if (tag == null)
                    {
                        var newTag = new Tag
                        {
                            Id = Guid.NewGuid(),
                            Name = name
                        };

                        tag = newTag;
                        db.Tags.Add(tag);
                    }

                    tags.Add(tag);
                }

                db.SaveChanges();
            }

            return tags;
        }

        public TagManager(Item item)
        {
            Item = item;
        }

        public TagManager(Guid itemId)
        {
            Item = Item.Get(itemId);
        }

        public Tag[] GetSimilarTagsByDescription()
        {
            using (var db = new Context())
            {
                var itemsWithSameDescription =
                    (from items in db.Items
                         // filter items that have tags
                     join itemTags in db.ItemTags on items.Id equals itemTags.ItemId
                     where
                        1 == 1
                        // Not the same item
                        && items.Id != Item.Id
                        // but same description
                        && items.Description == Item.Description
                     orderby items.PostedAt descending
                     select items);

                if (!itemsWithSameDescription.Any())
                {
                    return new Tag[0];
                }

                return (from tags in db.Tags
                        join itemTags in db.ItemTags on tags.Id equals itemTags.TagId
                        where itemTags.ItemId == itemsWithSameDescription.First().Id
                        select tags).ToArray();

            }
        }
    }
}
