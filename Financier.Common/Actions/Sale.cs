using System;

using Financier.Common.Models;

namespace Financier.Common.Actions
{
    public abstract class Sale : IAction
    {
        public DateTime At => SoldAt;

        public DateTime SoldAt { get; }

        public IProduct Product { get; }

        public Sale(IProduct product, DateTime soldAt)
        {
            Product = product;
            SoldAt = soldAt;
        }

        public decimal TotalBy(DateTime at)
        {
            return Product.TotalBy(at);
        }
    }
}
