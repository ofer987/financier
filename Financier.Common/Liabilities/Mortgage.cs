using System;
using System.Collections.Generic;
using System.Linq;

using Financier.Common.Models;

namespace Financier.Common.Liabilities
{
    public abstract class Mortgage : Liability<Home>, IMortgage
    {
        public IMonthlyPaymentCalculator Calculator { get; }

        public virtual decimal BaseValue { get; }
        public virtual decimal InitialValue => BaseValue;

        public DateTime InitiatedAt => Product.PurchasedAt;
        public int AmortisationPeriodInMonths { get; }
        public decimal InterestRate { get; }
        public decimal QuotedInterestRate => InterestRate;

        public abstract double PeriodicMonthlyInterestRate { get; }
        public double PeriodicAnnualInterestRate => PeriodicMonthlyInterestRate * 12;
        public double EffectiveAnnualInterestRate => Math.Pow(PeriodicMonthlyInterestRate + 1, 12) - 1;

        public virtual double MonthlyPayment => (Convert.ToDouble(BaseValue) * PeriodicMonthlyInterestRate) / (1 - Math.Pow(1 + PeriodicMonthlyInterestRate, - AmortisationPeriodInMonths));

        public Mortgage(Home product, IMonthlyPaymentCalculator calculator, decimal baseValue, decimal interestRate, int amortisationPeriodInMonths) : base(product)
        {
            Calculator = calculator;
            BaseValue = baseValue;
            AmortisationPeriodInMonths = amortisationPeriodInMonths;
            InterestRate = interestRate;
        }

        public IEnumerable<MonthlyPayment> GetMonthlyPayments(DateTime at)
        {
            return Calculator.GetMonthlyPayments(this, at);
        }

        public IEnumerable<decimal> GetPrincipalOnlyPayments(int year, int month, int day)
        {
            return Enumerable.Empty<decimal>();
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

        public bool IsMonthlyPayment(DateTime at)
        {
            return at.Day == InitiatedAt.Day;
        }
    }
}
