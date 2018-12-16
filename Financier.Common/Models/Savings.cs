using System;

using Financier.Common.Calculations;
using Financier.Common.Models.Expenses;
using Financier.Common.Extensions;

namespace Financier.Common.Models
{
    public abstract class Savings : IAsset
    {
        // TODO: place in configuration file or should be configurable somehow
        public const decimal YearlyInflationRate = 2.00M;

        // TODO place in a configuration file or pass in as constructor parameter
        public const decimal YearlyValuationRate = 5.00M;

        public double MonthlyInflationRate => Math.Pow(Convert.ToDouble(YearlyInflationRate) / 100, 1.0/12) - 1;

        public DateTime PurchasedAt { get; }

        public decimal PurchasePrice { get; }

        public virtual decimal QuotedInterestRate => 3.00M;

        public double EffectiveInterestRateMonthly => Math.Pow((Convert.ToDouble(QuotedInterestRate) / 100 + 1), 1.0/12) - 1;

        public Savings(DateTime purchasedAt, decimal purchasePrice)
        {
            PurchasedAt = purchasedAt;
            PurchasePrice = purchasePrice;
        }

        public decimal ValueAt(DateTime at)
        {
            return ValueAt(PurchasedAt.WholeMonthDifference(at));
        }

        public decimal ValueAt(int monthAfterInception)
        {
            if (monthAfterInception < 0)
            {
                throw new Exception($"{nameof(monthAfterInception)} cannot be negative number");
            }

            return PurchasePrice * Convert.ToDecimal(Math.Pow(EffectiveInterestRateMonthly, monthAfterInception));
        }

        public decimal ValueBy(DateTime at)
        {
            return ValueBy(PurchasedAt.WholeMonthDifference(at));
        }

        public decimal ValueBy(int monthAfterInception)
        {
            return ValueAt(monthAfterInception);
        }

        public decimal TotalBy(DateTime at)
        {
            return ValueBy(at);
        }
    }
}
