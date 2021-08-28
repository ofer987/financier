using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Financier.Common.Expenses.Models;

namespace Financier.Common.Expenses
{
    public class ItemTagger
    {
        public IEnumerable<string> ItemMatchers { get; }
        public string[] TagNames { get; }

        private Tag[] tags = new Tag[0];
        public Tag[] Tags
        {
            get
            {
                if (tags.Length == 0)
                {
                    this.tags = this.TagNames
                        .Select(Tag.GetOrCreate)
                        .ToArray();
                }

                return tags;
            }
        }

        public static void AddTagsToItems(IEnumerable<ItemTagger> taggings, IEnumerable<Guid> itemIds)
        {
            var items = itemIds.Select(Item.Get);
            foreach (var tagging in taggings)
            {
                foreach (var item in items)
                {
                    if (tagging.IsMatch(item.Description))
                    {
                        tagging.AddTags(item.Id);
                    }
                }
            }
        }

        public ItemTagger(string regularExpression, string[] tagNames)
        {
            this.ItemMatchers = new string[] { regularExpression };
            this.TagNames = tagNames;
        }

        public ItemTagger(IEnumerable<string> regularExpressions, string[] tagNames)
        {
            this.ItemMatchers = regularExpressions;
            this.TagNames = tagNames;
        }

        public bool IsMatch(string itemDescription)
        {
            return ItemMatchers
                .Select(item => new Regex(item, RegexOptions.IgnoreCase | RegexOptions.Compiled))
                .Any(regex => regex.IsMatch(itemDescription));
        }

        public void AddTags(Guid itemGuid)
        {
            var item = Item.Get(itemGuid);

            item.AddTags(Tags);
        }
    }
}
