using System.Linq;
using System.Collections.Generic;

namespace Financier.Common.Expenses.Models
{
    public class ItemResult
    {
        public ItemQuery Query { get; }
        public IEnumerable<Item> Items { get; }
        public decimal TotalAmount => Items.Aggregate(0.00M, (r, i) => r + i.TheRealAmount);

        public ItemResult(ItemQuery query, IEnumerable<Item> items)
        {
            Query = query;
            Items = items;
        }
    }
}
