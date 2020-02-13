using System;

namespace Financier.Common.Models
{
    public abstract class Liability : ILiability
    {
        protected Liability() {}

        public abstract decimal CostAt(int monthAfterInception);

        public virtual decimal CostAt(DateTime at)
        {
            return 0.00M;
            // return CostAt(at.SubtractWholeMonths(Product.PurchasedAt));
        }

        public abstract decimal CostBy(int monthAfterInception);

        public virtual decimal CostBy(DateTime at)
        {
            return 0.00M;
            // return CostBy(at.SubtractWholeMonths(Product.PurchasedAt));
        }
    }
}
