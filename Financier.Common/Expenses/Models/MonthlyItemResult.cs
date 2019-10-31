using System;
using System.Collections.Generic;

namespace Financier.Common.Expenses.Models
{
    public class MonthlyItemResult : ItemResult
    {
        public DateTime At { get; }
        public int Year => At.Year;
        public int Month => At.Month;

        public MonthlyItemResult(ItemQuery query, IEnumerable<Item> items, DateTime at) : base(query, items)
        {
            At = new DateTime(at.Year, at.Month, 1);
        }
    }
}
