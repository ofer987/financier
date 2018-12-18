using System;

using Financier.Common.Models;

namespace Financier.Common.Actions
{
    public abstract class Sell : IAction
    {
        public DateTime At => SoldAt;

        public DateTime SoldAt { get; }

        public IProduct Product { get; }

        public Sell(IProduct product, DateTime soldAt)
        {
            Product = product;
            SoldAt = soldAt;
        }

        // TODO rename to ConvertToCash ?????
        public decimal ConvertToCash(DateTime soldAt)
        {
            return Product.Sell(soldAt);
        }
    }
}
