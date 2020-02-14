using System;
using System.Linq;
using System.Collections.Generic;

namespace Financier.Common.Models
{
    public class SimpleProduct : Product
    {
        public decimal Price { get; }
        public SimpleProduct(string name, decimal price) : base(name)
        {
            Price = price;
        }

        public override IEnumerable<Money> GetValueAt(DateTime at)
        {
            yield return new Money(Price, at);
        }

        public override IEnumerable<Money> GetCostAt(DateTime _at)
        {
            return Enumerable.Empty<Money>();
        }
    }
}
