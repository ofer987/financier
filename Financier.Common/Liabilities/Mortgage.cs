using System;
using System.Collections.Generic;
using System.Linq;

using Financier.Common.Models;

namespace Financier.Common.Liabilities
{
    public abstract class Mortgage : IMortgage
    {
        // TODO: move functionality into Mortgage
        public IMonthlyPaymentCalculator Calculator { get; protected set; }

        private Money BaseValue { get; }
        public virtual Money InitialValue => BaseValue;

        public virtual DateTime InitiatedAt { get; }
        public virtual int AmortisationPeriodInMonths { get; }
        public virtual decimal InterestRate { get; }
        public decimal QuotedInterestRate => InterestRate;

        public abstract double PeriodicMonthlyInterestRate { get; }
        public decimal PeriodicAnnualInterestRate => QuotedInterestRate;
        public double EffectiveAnnualInterestRate => Math.Pow(PeriodicMonthlyInterestRate + 1, 12) - 1;

        public virtual double MonthlyPayment => (Convert.ToDouble(InitialValue) * PeriodicMonthlyInterestRate) / (1 - Math.Pow(1 + PeriodicMonthlyInterestRate, 0 - AmortisationPeriodInMonths));

        public Mortgage(IMonthlyPaymentCalculator calculator, Money baseValue, decimal interestRate, int amortisationPeriodInMonths, DateTime initiatedAt) : this(baseValue, interestRate, amortisationPeriodInMonths, initiatedAt)
        {
            Calculator = calculator;
        }

        public Mortgage(Money baseValue, decimal interestRate, int amortisationPeriodInMonths, DateTime initiatedAt)
        {
            Calculator = new MonthlyPaymentCalculator();
            BaseValue = baseValue;
            AmortisationPeriodInMonths = amortisationPeriodInMonths;
            InterestRate = interestRate;
            InitiatedAt = initiatedAt;
        }

        protected Mortgage()
        {
        }

        public Money GetBalance(DateTime at)
        {
            return GetMonthlyPayments(at)
                .Select(payment => payment.Balance)
                .Last();
        }

        public IEnumerable<MonthlyPayment> GetMonthlyPayments()
        {
            return Calculator.GetMonthlyPayments(this);
        }

        public IEnumerable<MonthlyPayment> GetMonthlyPayments(DateTime endAt)
        {
            return Calculator.GetMonthlyPayments(this, endAt);
        }

        public virtual IEnumerable<decimal> GetPrincipalOnlyPayments(int year, int month, int day)
        {
            return Enumerable.Empty<decimal>();
        }

        public bool IsMonthlyPayment(DateTime at)
        {
            return at.Day == InitiatedAt.Day;
        }
    }
}
