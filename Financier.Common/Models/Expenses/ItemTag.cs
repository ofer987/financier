using System;

namespace Financier.Common.Models.Expenses
{
    public class ItemTag
    {
        public Guid Id { get; set; }

        public Item Item { get; set; }

        public Tag Tag { get; set; }
    }
}
