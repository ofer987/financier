using System.Linq;
using System.Collections.Generic;

using Financier.Common.Expenses.Models;

namespace Financier.Common.Expenses
{
    public class ItemResult
    {
        public ItemQuery Query { get; }
        public IEnumerable<Item> Items { get; }
        public decimal Amount => Items.Aggregate(0.00M, (r, i) => r + i.TheRealAmount);

        public ItemResult(ItemQuery query, IEnumerable<Item> items)
        {
            Query = query;
            Items = items;
        }
    }
}
