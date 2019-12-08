using System;

using Financier.Common.Models;

namespace Financier.Common.Calculations
{
    public class Mortgage : Liability
    {
        public decimal DownPayment { get; }
        public decimal BaseValue { get; }
        public virtual decimal Value { get; }
        public int AmortisationPeriodInMonths { get; }
        public decimal InterestRate { get; }
        public decimal QuotedInterestRate => InterestRate;

        public double EffectiveInterestRateMonthly => Math.Pow(Math.Pow((Convert.ToDouble(QuotedInterestRate) / (2 * 100) + 1), 2), 1.0/12) - 1;
        public double AnnualPercentageRateAnnual => EffectiveInterestRateMonthly * 12;

        public Mortgage(IProduct product, decimal downPayment, decimal baseValue, decimal interestRate, int amortisationPeriodInMonths) : base(product)
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

            var val = Convert.ToDecimal(Convert.ToDouble(Value) * effectiveInterestRateMonthly / ( 1 - Math.Pow((1 + effectiveInterestRateMonthly), -1 * AmortisationPeriodInMonths)));

            return decimal.Round(val, 2);
        }

        public decimal GetMonthlyInterestPayment(int monthAfterInception)
        {
            if (monthAfterInception < 0)
            {
                throw new Exception($"{nameof(monthAfterInception)} cannot be negative number");
            }

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

        public decimal GetInterestPaymentsBy(int monthAfterInception)
        {
            if (monthAfterInception < 0)
            {
                throw new Exception($"{nameof(monthAfterInception)} cannot be negative number");
            }

            var monthlyPayment = GetMonthlyPayment();
            var balanceAtMonth = Value;
            var interestPayment = 0.00M;
            var totalInterestPayments = 0.00M;
            var effectiveInterestRateMonthly = EffectiveInterestRateMonthly;

            for (var i = 0; i < monthAfterInception; i += 1)
            {
                interestPayment = Convert.ToDecimal(Convert.ToDouble(balanceAtMonth) * effectiveInterestRateMonthly / 100);
                totalInterestPayments += interestPayment;
                var principalPayment = monthlyPayment - interestPayment;

                balanceAtMonth -= principalPayment;
            }

            return totalInterestPayments;
        }

        public decimal GetBalance(int monthAfterInception)
        {
            if (monthAfterInception < 0)
            {
                throw new Exception($"{nameof(monthAfterInception)} cannot be negative number");
            }

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

        public override decimal CostAt(int monthAfterInception)
        {
            return GetMonthlyPayment();
        }

        public override decimal CostBy(int monthAfterInception)
        {
            return GetMonthlyPayment() * monthAfterInception;
        }
    }
}
