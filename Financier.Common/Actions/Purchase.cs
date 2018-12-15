using System;

using Financier.Common.Models;

namespace Financier.Common.Actions
{
    public abstract class Purchase
    {
        public DateTime PurchasedAt => Product.PurchasedAt;

        public IProduct Product { get; }

        public Base(IProduct product)
        {
            Product = product;
        }
    }
}
