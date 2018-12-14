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

        public double EffectiveInterestRateMonthly => Math.Pow(Math.Pow((Convert.ToDouble(QuotedInterestRate) / (2 * 100) + 1), 2), 1.0/12) - 1;

        public double AnnualPercentageRateAnnual => EffectiveInterestRateMonthly * 12;

        public Mortgage(decimal downPayment, decimal baseValue, int amortisationPeriodInMonths, decimal interestRate)
        {
            DownPayment = DownPayment;
            BaseValue = baseValue;
            AmortisationPeriodInMonths = amortisationPeriodInMonths;
            InterestRate = interestRate;
        }

        // fixed rate calculations
        // TODO create a variable-rate calculation
        public decimal GetMonthlyPayment()
        {
            var effectiveInterestRateMonthly = EffectiveInterestRateMonthly;

            return Convert.ToDecimal(Convert.ToDouble(Value) * effectiveInterestRateMonthly / ( 1 - Math.Pow((1 + effectiveInterestRateMonthly), -1 * AmortisationPeriodInMonths)));
        }

        // TODO incomplete
        public decimal GetMonthlyInterestPayment(int monthAfterInception)
        {
            var monthlyPayment = GetMonthlyPayment();
            var balanceAtMonth = Value;
            var interestPayment = 0.00M;
            var effectiveInterestRateMonthly = EffectiveInterestRateMonthly;

            for (var i = 0; i < monthAfterInception; i += 1)
            {
                interestPayment = Convert.ToDecimal(Convert.ToDouble(balanceAtMonth) * effectiveInterestRateMonthly / 100);
                var principalPayment = monthlyPayment - interestPayment;

                balanceAtMonth -= principalPayment;
            }

            return interestPayment;
        }

        public decimal GetBalance(int monthAfterInception)
        {
            var monthlyPayment = GetMonthlyPayment();
            var balanceAtMonth = Value;
            var effectiveInterestRateMonthly = EffectiveInterestRateMonthly;

            for (var i = 0; i < monthAfterInception; i += 1)
            {
                var interestPayment = Convert.ToDecimal(Convert.ToDouble(balanceAtMonth) * effectiveInterestRateMonthly / 100);
                var principalPayment = monthlyPayment - interestPayment;

                balanceAtMonth -= principalPayment;
            }

            return balanceAtMonth;
        }
    }
}
