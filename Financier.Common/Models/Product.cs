using System;
using System.Collections.Generic;
using System.Linq;

namespace Financier.Common.Models
{
    public abstract class Product : IProduct
    {
        // TODO: place in configuration file or should be configurable somehow
        public const decimal YearlyInflationRate = 2.00M;

        // TODO place in a configuration file or pass in as constructor parameter
        public const decimal YearlyValuationRate = 5.00M;

        public double MonthlyInflationRate => Math.Pow(Convert.ToDouble(YearlyInflationRate) / 100, 1.0/12) - 1;

        private DateTime? soldAt = null;
        public DateTime SoldAt
        {
            get
            {
                if (!soldAt.HasValue)
                {
                    throw new Exception("Product has not been sold yet");
                }

                return soldAt.Value;
            }

            private set
            {
                if (value < PurchasedAt)
                {
                    throw new Exception($"Product (insert identifier) cannot be sold at ({value}) before it was purchased at ({PurchasedAt})");
                }

                soldAt = value;
            }
        }

        private decimal? sellPrice = null;
        public decimal SellPrice
        {
            get
            {
                if (!sellPrice.HasValue)
                {
                    throw new Exception("Product has not been sold yet");
                }

                return sellPrice.Value;
            }

            private set
            {
                sellPrice = value;
            }
        }

        public bool IsSold => soldAt.HasValue;

        public DateTime PurchasedAt { get; }

        public List<IAsset> Assets { get; } = new List<IAsset>();

        public List<ILiability> Liabilities { get; } = new List<ILiability>();

        public Product(DateTime purchasedAt)
        {
            PurchasedAt = purchasedAt;
        }

        public virtual void Sell(decimal price, DateTime at)
        {
            SoldAt = at;
            SellPrice = price;
        }

        public virtual decimal ValueBy(DateTime at)
        {
            if (IsSold && SoldAt < at)
            {
                at = SoldAt;
            }

            return Assets
                .Select(asset => asset.ValueBy(at))
                .Aggregate(0.00M, (total, val) => total += val);
        }

        public virtual decimal CostBy(DateTime at)
        {
            if (IsSold && SoldAt < at)
            {
                at = SoldAt;
            }

            return Liabilities
                .Select(liability => liability.CostBy(at))
                .Aggregate(0.00M, (total, cost) => total += cost);
        }
    }
}
