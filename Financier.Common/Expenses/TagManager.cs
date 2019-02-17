using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

using Financier.Common.Extensions;
using Financier.Common.Expenses.Models;

namespace Financier.Common.Expenses
{
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
                        .DefaultIfEmpty(null)
                        .FirstOrDefault(t => t.Name == name);

                    if (tag == null)
                    {
                        var newTag = new Tag
                        {
                            Id = Guid.NewGuid(),
                               Name = name
                        };

                        tag = newTag;
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

        public List<Tag> AddTags(IEnumerable<Tag> newTags)
        {
            using (var db = new Context())
            {
                var existingItemTags = db.ItemTags
                    .Include(it => it.Tag)
                    .Where(it => it.ItemId == Item.Id);
                var existingTags = existingItemTags.Select(it => it.Tag);

                foreach (var newTag in newTags)
                {
                    if (!existingTags.Any(tag => tag.Name == newTag.Name))
                    {
                        var itemTag = new ItemTag
                        {
                            ItemId = Item.Id,
                            TagId = newTag.Id
                        };

                        db.ItemTags.Add(itemTag);
                    }
                }

                db.SaveChanges();

                // Or should represent only the tags that have been added
                return (from tags in db.Tags
                        join itemTags in db.ItemTags on tags.Id equals itemTags.TagId
                        where itemTags.ItemId == Item.Id
                        select tags).ToList();
            }
        }

        public Tag[] GetSimilarTagsByDescription()
        {
            using (var db = new Context())
            {
                var itemsWithSameDescription =
                    (from items in db.Items
                     where
                        1 == 1
                        && items.Id != Item.Id
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
