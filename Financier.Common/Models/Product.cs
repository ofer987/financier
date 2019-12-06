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

        public Guid Id { get; }

        public double MonthlyInflationRate => Math.Pow(Convert.ToDouble(YearlyInflationRate) / 100, 1.0/12) - 1;

        public double MonthlyValuationRate => Math.Pow(Convert.ToDouble(YearlyValuationRate) / 100, 1.0/12) - 1;

        public string Name { get; }

        public decimal InvestmentPrice => Assets.Aggregate(0.00M, (total, asset) => total += asset.InvestmentPrice);

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

        public decimal PurchasePrice 
        {
            get
            {
                return Assets
                    .Select(asset => asset.PurchasePrice)
                    .Aggregate(0.00M, (total, price) => total += price);
            }
        }

        public List<IAsset> Assets { get; } = new List<IAsset>();

        public List<ILiability> Liabilities { get; } = new List<ILiability>();

        public Product(string name, DateTime purchasedAt)
        {
            Id = Guid.NewGuid();
            Name = name;
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

        public override string ToString()
        {
            return $"Product ({nameof(Id)} = {Id})";
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var other = obj as Product;
            if (other == null)
            {
                return false;
            }

            if (Id != other.Id)
            {
                return false;
            }

            return true;
        }

        public static bool operator ==(Product x, Product y)
        {
            if (object.ReferenceEquals(x, null))
            {
                return (object.ReferenceEquals(y, null));
            }

            return x.Equals(y);
        }

        public static bool operator !=(Product x, Product y)
        {
            return !(x == y);
        }
    }
}
