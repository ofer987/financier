using System;

using Financier.Common.Models;

namespace Financier.Common.Calculations
{
    public class Mortgage
    {
        public decimal DownPayment { get; }

        public decimal BaseValue { get; }

        public virtual decimal Value { get; }

        public int AmortisationPeriodInMonths { get; }

        public decimal InterestRate { get; }

        public decimal QuotedInterestRate => InterestRate;

        public double EffectiveAnnualRate => Math.Pow(Math.Pow((Convert.ToDouble(QuotedInterestRate) / (2 * 100) + 1), 2), 1.0/12) - 1;

        public Mortgage(decimal downPayment, decimal baseValue, int amortisationPeriodInMonths, decimal interestRate)
        {
            DownPayment = DownPayment;
            BaseValue = baseValue;
            AmortisationPeriodInMonths = amortisationPeriodInMonths;
            InterestRate = interestRate;
        }

        // fixed rate calculations
        // TODO create a variable-rate calculation
        public decimal MonthlyPayment()
        {
            var effectiveAnnualRate = EffectiveAnnualRate;

            return Convert.ToDecimal(Convert.ToDouble(Value) * effectiveAnnualRate / ( 1 - Math.Pow((1 + effectiveAnnualRate), -1 * AmortisationPeriodInMonths)));
        }
    }
}
