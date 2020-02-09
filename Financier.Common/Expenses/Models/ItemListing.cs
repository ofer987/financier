using System;
using System.Collections.Generic;
using System.Linq;

namespace Financier.Common.Expenses.Models
{
    public class ItemListing
    {
        public static ItemListing operator +(ItemListing first, ItemListing second)
        {
            return new ItemListing(
                first.Tags.Concat(second.Tags),
                first.Items.Concat(second.Items)
            );
        }

        public decimal Amount => Items.Aggregate(0.00M, (r, i) => r + i.TheRealAmount);
        public double AccuratePercentage => Convert.ToDouble(Amount) / Convert.ToDouble(TotalAmount);
        public decimal Percentage => Amount / TotalAmount;
        public IEnumerable<Tag> Tags { get; set; }
        public IEnumerable<Item> Items { get; set; }
        public decimal TotalAmount { get; set; }

        public ItemListing()
        {
            Tags = Enumerable.Empty<Tag>();
            Items = Enumerable.Empty<Item>();
        }

        public ItemListing(IEnumerable<Tag> tags, IEnumerable<Item> items)
        {
            Tags = tags;
            Items = items;
        }

        public ItemListing(string tagNames, IEnumerable<Item> items)
        {
            Tags = tagNames
                .Split(",")
                .Select(tagName => tagName.Trim())
                .Select(tagName => new Tag { Name = tagName });

            Items = items;
        }
    }
}
