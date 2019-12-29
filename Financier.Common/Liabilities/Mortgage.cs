using System;
using System.Collections.Generic;
using System.Linq;

using Financier.Common.Models;

namespace Financier.Common.Liabilities
{
    public abstract class Mortgage : Liability<Home>, IMortgage
    {
        public virtual decimal BaseValue { get; }
        public virtual decimal InitialValue => BaseValue;
        public int AmortisationPeriodInMonths { get; }
        public decimal InterestRate { get; }
        public decimal QuotedInterestRate => InterestRate;

        public abstract double PeriodicMonthlyInterestRate { get; }
        public double PeriodicAnnualInterestRate => PeriodicMonthlyInterestRate * 12;
        public double EffectiveAnnualInterestRate => Math.Pow(PeriodicMonthlyInterestRate + 1, 12) - 1;

        public virtual double MonthlyPayment => (Convert.ToDouble(BaseValue) * PeriodicMonthlyInterestRate) / (1 - Math.Pow(1 + PeriodicMonthlyInterestRate, - AmortisationPeriodInMonths));

        public Mortgage(Home product, decimal baseValue, decimal interestRate, int amortisationPeriodInMonths) : base(product)
        {
            BaseValue = baseValue;
            AmortisationPeriodInMonths = amortisationPeriodInMonths;
            InterestRate = interestRate;
        }

        public virtual IEnumerable<decimal> GetMonthlyInterestPayments(int monthAfterInception)
        {
            if (monthAfterInception <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(monthAfterInception), "Should be greater than 0");
            }

            var monthlyPayment = Convert.ToDecimal(MonthlyPayment);
            var balance = InitialValue;
            var interestRate = PeriodicAnnualInterestRate;

            for (var i = 0; i < monthAfterInception; i += 1)
            {
                var interestPayment = Convert.ToDecimal(Convert.ToDouble(balance) * interestRate / 12);

                var principalPayment = monthlyPayment - interestPayment;
                balance -= decimal.Round(Convert.ToDecimal(principalPayment), 2);

                yield return decimal.Round(interestPayment, 2);
            }
        }

        public decimal GetMonthlyInterestPayment(int monthAfterInception)
        {
            return GetMonthlyInterestPayments(monthAfterInception)
                .Last();
        }

        public decimal GetTotalInterestPayment(int monthAfterInception)
        {
            return GetMonthlyInterestPayments(monthAfterInception)
                .Sum();
        }

        public virtual IEnumerable<decimal> GetMonthlyPrincipalPayments(int monthAfterInception)
        {
            if (monthAfterInception < 0)
            {
                throw new Exception($"{nameof(monthAfterInception)} cannot be negative number");
            }

            var monthlyPayment = Convert.ToDecimal(MonthlyPayment);
            var balance = InitialValue;
            var interestRate = PeriodicAnnualInterestRate;

            for (var i = 0; i < monthAfterInception; i += 1)
            {
                var interestPayment = Convert.ToDecimal(Convert.ToDouble(balance) * interestRate / 12);
                var principalPayment = monthlyPayment - interestPayment;

                balance -= principalPayment;

                yield return decimal.Round(principalPayment, 2);
            }
        }

        public decimal GetTotalPrincipalPayment(int monthAfterInception)
        {
            return GetMonthlyPrincipalPayments(monthAfterInception)
                .Sum();
        }

        public decimal GetMonthlyPrincipalPayment(int monthAfterInception)
        {
            return GetMonthlyPrincipalPayments(monthAfterInception)
                .Last();
        }

        public virtual decimal GetBalance(int monthAfterInception)
        {
            if (monthAfterInception <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(monthAfterInception), "Should be greater than 0");
            }

            var monthlyPayment = Convert.ToDecimal(MonthlyPayment);
            var balance = InitialValue;
            var interestRate = PeriodicAnnualInterestRate;

            for (var i = 0; i < monthAfterInception; i += 1)
            {
                var interestPayment = Convert.ToDecimal(Convert.ToDouble(balance) * interestRate / 12);
                var principalPayment = monthlyPayment - interestPayment;

                balance -= principalPayment;
            }

            return decimal.Round(balance, 2);
        }

        public override decimal CostAt(int monthAfterInception)
        {
            if (monthAfterInception <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(monthAfterInception), "Should be greater than 0");
            }

            return Convert.ToDecimal(MonthlyPayment);
        }

        public override decimal CostBy(int monthAfterInception)
        {
            if (monthAfterInception <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(monthAfterInception), "Should be greater than 0");
            }

            return Convert.ToDecimal(MonthlyPayment) * monthAfterInception;
        }
    }
}
