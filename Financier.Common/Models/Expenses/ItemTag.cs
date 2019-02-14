using System;

namespace Financier.Common.Models.Expenses
{
    public class ItemTag
    {
        public Guid ItemId { get; set; }

        public Item Item { get; set; }

        public Guid TagId { get; set; }

        public Tag Tag { get; set; }
    }
}
