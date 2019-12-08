using System;
using System.Collections.Generic;

namespace Financier.Common.Models
{
    public abstract class Product : IProduct
    {
        // TODO: place in configuration file or should be configurable somehow
        // TODO place in a configuration file or pass in as constructor parameter
        public const decimal YearlyInflationRate = 2.00M;
        public const decimal YearlyValuationRate = 5.00M;

        public double MonthlyInflationRate => Math.Pow(Convert.ToDouble(YearlyInflationRate) / 100, 1.0/12) - 1;
        public double MonthlyValuationRate => Math.Pow(Convert.ToDouble(YearlyValuationRate) / 100, 1.0/12) - 1;

        public Guid Id { get; }
        public string Name { get; }

        // What to do with these?
        public List<IAsset> Assets { get; } = new List<IAsset>();
        public List<ILiability> Liabilities { get; } = new List<ILiability>();

        protected Product(string name)
        {
            Id = Guid.NewGuid();
            Name = name;
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
