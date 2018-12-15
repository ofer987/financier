using System;

using Financier.Common.Models;

namespace Financier.Common.Actions
{
    public abstract class Sale
    {
        public DateTime SoldAt { get; }

        public IProduct Product { get; }

        public Sale(IProduct product, DateTime soldAt)
        {
            Product = product;
            SoldAt = soldAt;
        }
    }
}
