using System;

namespace Financier.Common.Models
{
    public abstract class Asset<T> : IAsset where T : IProduct
    {
        public T Product { get; }
        public decimal PurchasePrice { get; }

        public Asset(T product, decimal purchasePrice)
        {
            Product = product;
            PurchasePrice = purchasePrice;
        }

        public virtual decimal ValueAt(DateTime at)
        {
            return PurchasePrice;
            // return ValueAt(at.SubtractWholeMonths(Product.PurchasedAt));
        }

        public abstract decimal ValueAt(int monthAfterInception);

        public virtual decimal ValueBy(DateTime at)
        {
            return PurchasePrice;
            // return ValueBy(at.SubtractWholeMonths(Product.PurchasedAt));
        }

        public abstract decimal ValueBy(int monthAfterInception);
    }
}
