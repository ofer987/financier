using System;

using Financier.Common.Models;

namespace Financier.Common.Actions
{
    public abstract class Purchase : IAction
    {
        public DateTime At => PurchasedAt;

        public DateTime PurchasedAt => Product.PurchasedAt;

        public IProduct Product { get; }

        public Purchase(IProduct product)
        {
            Product = product;
        }

        public decimal TotalBy(DateTime at)
        {
            return -Product.TotalBy(at);
        }
    }
}
