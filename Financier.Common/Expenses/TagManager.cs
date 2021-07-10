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
                        select tags).AsEnumerable().ToList();
            }
        }

        public bool UpdateTags(IEnumerable<string> newTagNames)
        {
            var newTags = newTagNames.Select(name => Tag.GetOrCreate(name));

            return UpdateTags(newTags);
        }

        public bool UpdateTags(IEnumerable<Tag> newTags)
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

                // var itemTagsToDelete = existingTags
                //     .Reject(existingTag => newTags.Any(newTag => newTag.Name == existingTag.Name));

                var itemTagsToDelete = existingItemTags
                    .Reject(existingItemTag => newTags.Any(newTag => newTag.Name == existingItemTag.Tag.Name));

                foreach (var itemTag in itemTagsToDelete)
                {
                    db.ItemTags.Remove(itemTag);
                }

                db.SaveChanges();
            }

            return true;
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
