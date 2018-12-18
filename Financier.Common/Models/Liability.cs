using System;

using Financier.Common.Extensions;

namespace Financier.Common.Models
{
    public abstract class Liability : ILiability
    {
        public IProduct Product { get; }

        public Liability(IProduct product)
        {
            Product = product;
        }

        public abstract decimal CostAt(int monthAfterInception);

        public virtual decimal CostAt(DateTime at)
        {
            return CostAt(at.SubtractWholeMonths(Product.PurchasedAt));
        }

        public abstract decimal CostBy(int monthAfterInception);

        public virtual decimal CostBy(DateTime at)
        {
            return CostBy(at.SubtractWholeMonths(Product.PurchasedAt));
        }
    }
}
