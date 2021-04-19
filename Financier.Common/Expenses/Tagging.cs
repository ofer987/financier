using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Financier.Common.Extensions;
using Financier.Common.Expenses.Models;

namespace Financier.Common.Expenses
{
    public class Tagging
    {
        public string RegularExpression { get; }
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

        public static void AddTagsToItems(IEnumerable<Tagging> taggings, IEnumerable<Guid> itemIds)
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

        public Tagging(string regularExpression, string[] tagNames)
        {
            this.RegularExpression = regularExpression;
            this.TagNames = tagNames;
        }

        public bool IsMatch(string itemDescription)
        {
            var matcher = new Regex(RegularExpression, RegexOptions.IgnoreCase | RegexOptions.Compiled);

            return matcher.IsMatch(itemDescription);
        }

        public List<Tag> AddTags(Guid itemGuid)
        {
            var item = Item.Get(itemGuid);

            return new TagManager(itemGuid).AddTags(this.Tags);
        }
    }
}
