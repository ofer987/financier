using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

using Financier.Common.Extensions;
using Financier.Common.Models.Expenses;

namespace Financier.Common
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

            using (var db = new ExpensesContext())
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

        public bool TryAddTags(IEnumerable<Tag> newTags, out List<Tag> itemTags)
        {
            // Or should represent only the tags that have been added
            itemTags = new List<Tag>();

            using (var db = new ExpensesContext())
            {
                db.Tags
                    .Include(tag => tag.ItemTags)
                    .ThenInclude(it => it.Item)
                    .Where(Expression<Func<Tag, bool>> predicate)
                    .Include(tag => tag.)
                    .Where(tag => tag.)
                db.
            }
            return true;
        }
    }
}
