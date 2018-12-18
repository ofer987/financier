using System;

using Financier.Common.Extensions;

namespace Financier.Common.Models
{
    public abstract class Asset : IAsset
    {
        public IProduct Product { get; }

        public decimal PurchasePrice { get; }

        public virtual bool IsSold => Product.IsSold;

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

        public Asset(IProduct product, decimal purchasePrice)
        {
            Product = product;
            PurchasePrice = purchasePrice;
        }

        public virtual decimal ValueAt(DateTime at)
        {
            return ValueAt(at.SubtractWholeMonths(Product.PurchasedAt));
        }

        public abstract decimal ValueAt(int monthAfterInception);

        public virtual decimal ValueBy(DateTime at)
        {
            return ValueBy(at.SubtractWholeMonths(Product.PurchasedAt));
        }

        public abstract decimal ValueBy(int monthAfterInception);
    }
}
