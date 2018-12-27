using System;

namespace Financier.Common.Models
{
    public abstract class Savings : Asset
    {
        // TODO: place in configuration file or should be configurable somehow
        public const decimal YearlyInflationRate = 2.00M;

        // TODO place in a configuration file or pass in as constructor parameter
        public const decimal YearlyValuationRate = 5.00M;

        // TODO define amortisation period, or is that what it is called ??
        // TODO maybe it is called the compounding period; not sure.
        public double MonthlyInflationRate => Math.Pow(Convert.ToDouble(YearlyInflationRate) / 100, 1.0/12) - 1;

        public DateTime PurchasedAt { get; }

        public DateTime SoldAt { get; set; }

        public virtual decimal QuotedInterestRate => 3.00M;

        public double EffectiveInterestRateMonthly => Math.Pow((1.0 + Convert.ToDouble(QuotedInterestRate / 100)), 1.0/12) - 1.0;

        public Savings(IProduct product, decimal purchasePrice) : base(product, purchasePrice)
        {
        }

        public override decimal ValueAt(int monthAfterInception)
        {
            if (monthAfterInception < 0)
            {
                throw new Exception($"{nameof(monthAfterInception)} cannot be negative number");
            }

            return PurchasePrice * Convert.ToDecimal(Math.Pow(1 + EffectiveInterestRateMonthly, monthAfterInception));
        }

        public override decimal ValueBy(int monthAfterInception)
        {
            return ValueAt(monthAfterInception);
        }

        public decimal TotalBy(DateTime at)
        {
            return ValueBy(at);
        }
    }
}
