using System;

namespace Financier.Common.Models
{
    public abstract class Asset<T> : IAsset where T : IProduct
    {
        public T Product { get; }

        public decimal PurchasePrice { get; }

        public virtual decimal InvestmentPrice => PurchasePrice;

        public virtual bool IsSold => true;
        // public virtual bool IsSold => Product.IsSold;

        private decimal? sellPrice = null;
        public virtual decimal SellPrice
        {
            get
            {
                if (!sellPrice.HasValue)
                {
                    throw new Exception("Product has not been sold yet");
                }

                return sellPrice.Value;
            }

            set
            {
                sellPrice = value;
            }
        }

        public Asset(T product, decimal purchasePrice)
        {
            Product = product;
            PurchasePrice = purchasePrice;
        }

        public virtual decimal ValueAt(DateTime at)
        {
            return 0.00M;
            // return ValueAt(at.SubtractWholeMonths(Product.PurchasedAt));
        }

        public abstract decimal ValueAt(int monthAfterInception);

        public virtual decimal ValueBy(DateTime at)
        {
            return 0.00M;
            // return ValueBy(at.SubtractWholeMonths(Product.PurchasedAt));
        }

        public abstract decimal ValueBy(int monthAfterInception);
    }
}
