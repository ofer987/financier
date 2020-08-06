using System;
using System.Collections.Generic;

namespace Financier.Common.Models
{
    public class SimpleProduct : Product
    {
        public SimpleProduct(string name, decimal price) : base(name, price)
        {
        }

        public override IEnumerable<decimal> GetValueAt(DateTime _at)
        {
            yield return Price;
        }

        public override IEnumerable<decimal> GetCostAt(DateTime _at)
        {
            yield break;
        }
    }
}
