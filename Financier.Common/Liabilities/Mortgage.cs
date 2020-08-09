using System;
using System.Collections.Generic;
using System.Linq;

using Financier.Common.Expenses.Actions;
using Financier.Common.Extensions;

namespace Financier.Common.Liabilities
{
    public abstract class Mortgage : IMortgage
    {
        // TODO: move functionality into Mortgage
        public IMonthlyPaymentCalculator Calculator { get; protected set; }

        public virtual Guid Id { get; }
        public virtual string Name => string.Empty;

        public decimal Price => 0.00M - InitialValue;

        private decimal BaseValue { get; }
        public virtual decimal InitialValue => BaseValue;

        private DateTime initiatedAt;
        public virtual DateTime InitiatedAt
        {
            get
            {
                return initiatedAt;
            }

            set
            {
                initiatedAt = value.GetDate();
            }
        }

        public virtual int AmortisationPeriodInMonths { get; }
        public virtual decimal InterestRate { get; }
        public decimal QuotedInterestRate => InterestRate;

        public abstract double PeriodicMonthlyInterestRate { get; }
        public decimal PeriodicAnnualInterestRate => QuotedInterestRate;
        public double EffectiveAnnualInterestRate => Math.Pow(PeriodicMonthlyInterestRate + 1, 12) - 1;

        public virtual double MonthlyPayment => (Convert.ToDouble(InitialValue) * PeriodicMonthlyInterestRate) / (1 - Math.Pow(1 + PeriodicMonthlyInterestRate, 0 - AmortisationPeriodInMonths));

        protected Mortgage(IMonthlyPaymentCalculator calculator, decimal baseValue, decimal interestRate, int amortisationPeriodInMonths, DateTime initiatedAt) : this(baseValue, interestRate, amortisationPeriodInMonths, initiatedAt)
        {
            Calculator = calculator;
        }

        protected Mortgage(decimal baseValue, decimal interestRate, int amortisationPeriodInMonths, DateTime initiatedAt) : this()
        {
            Calculator = new MonthlyPaymentCalculator();
            BaseValue = baseValue;
            AmortisationPeriodInMonths = amortisationPeriodInMonths;
            InterestRate = interestRate;
            InitiatedAt = initiatedAt;
        }

        protected Mortgage()
        {
            Id = Guid.NewGuid();
        }

        public decimal GetBalance(DateTime at)
        {
            return GetMonthlyPayments(at)
                .Select(payment => payment.Balance)
                .Last();
        }

        public IEnumerable<Payment> GetMonthlyPayments()
        {
            return Calculator.GetMonthlyPayments(this);
        }

        public IEnumerable<Payment> GetMonthlyPayments(DateTime endAt)
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

        public IEnumerable<decimal> GetValueAt(DateTime at)
        {
            // TODO: why is this empty?
            return Enumerable.Empty<decimal>();
        }

        public IEnumerable<decimal> GetCostAt(DateTime at)
        {
            return GetMonthlyPayments(at)
                .Select(payment => payment.Amount)
                .Select(amount => amount.Value);
        }

        public virtual decimal GetPurchasePrice(decimal _price)
        {
            return new MortgagePurchaseStrategy().GetReturnedPrice();
        }

        public virtual decimal GetSalePrice(decimal _price, DateTime at)
        {
            // TODO: Announce whether or not we are trading this home for another one
            return new MortgageSaleStrategy(GetBalance(at), true).GetReturnedPrice();
        }
    }
}
