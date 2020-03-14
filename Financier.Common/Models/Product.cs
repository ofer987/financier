using System;
using System.Text;
using System.Collections.Generic;

using Financier.Common.Expenses.Actions;

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

        public Money Price { get; }

        protected Product(string name, Money price)
        {
            Id = Guid.NewGuid();
            Name = name;

            Price = price;
        }

        public abstract IEnumerable<Money> GetValueAt(DateTime at);
        public abstract IEnumerable<Money> GetCostAt(DateTime at);

        public IPurchaseStrategy GetPurchaseStrategy();

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"{GetType().Name}:");
            sb.AppendLine($"\t{nameof(Name)}: ({Name})");
            sb.AppendLine($"\t{nameof(Id)}: ({Id})");

            return sb.ToString();
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
