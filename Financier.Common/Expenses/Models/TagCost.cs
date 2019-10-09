using System.Collections.Generic;
using System.Linq;

namespace Financier.Common.Expenses.Models
{
    public class TagCost
    {
        public static TagCost operator +(TagCost first, TagCost second)
        {
            return new TagCost(
                first.Tags.Concat(second.Tags),
                first.Items.Concat(second.Items)
            );
        }

        public decimal Amount => Items.Aggregate(0.00M, (r, i) => r + i.Amount);
        public double Percentage { get; set; }
        public IEnumerable<Tag> Tags { get; set; }
        public IEnumerable<Item> Items { get; set; }

        public TagCost()
        {
            Tags = Enumerable.Empty<Tag>();
            Items = Enumerable.Empty<Item>();
        }

        public TagCost(IEnumerable<Tag> tags, IEnumerable<Item> items)
        {
            Tags = tags;
            Items = items;
        }
    }
}
