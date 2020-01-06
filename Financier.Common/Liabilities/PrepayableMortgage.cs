using System;
using System.Collections.Generic;
using System.Linq;

namespace Financier.Common.Liabilities
{
    public class PrepayableMortgage : IMortgage, IPrepayable
    {
        public IMortgage BaseMortgage { get; }
        public CappedPayments Prepayments { get; }
        public IMonthlyPaymentCalculator Calculator { get; }

        public decimal BaseValue => BaseMortgage.BaseValue;
        public decimal InitialValue => BaseMortgage.InitialValue;
        public DateTime InitiatedAt => BaseMortgage.InitiatedAt;

        public int AmortisationPeriodInMonths => BaseMortgage.AmortisationPeriodInMonths;
        public decimal InterestRate => BaseMortgage.InterestRate;
        public decimal QuotedInterestRate => BaseMortgage.QuotedInterestRate;

        public double PeriodicMonthlyInterestRate => BaseMortgage.PeriodicMonthlyInterestRate;
        public double PeriodicAnnualInterestRate => BaseMortgage.PeriodicAnnualInterestRate;
        public double EffectiveAnnualInterestRate => BaseMortgage.EffectiveAnnualInterestRate;

        public decimal MaximumAllowedPrepaymentTotal => Prepayments.MaximumAnnualTotal;
        public double MonthlyPayment => BaseMortgage.MonthlyPayment;

        public PrepayableMortgage(IMortgage baseMortgage, IMonthlyPaymentCalculator calculator, decimal maximumAllowedPrepaymentPercentage = 0.10M)
        {
            BaseMortgage = baseMortgage;
            Calculator = calculator;
            Prepayments = new CappedPayments(InitialValue * maximumAllowedPrepaymentPercentage);
        }

        public PrepayableMortgage(IMortgage baseMortgage, decimal maximumAllowedPrepaymentPercentage = 0.10M)
        {
            BaseMortgage = baseMortgage;
            Calculator = new MonthlyPaymentCalculator();
            Prepayments = new CappedPayments(InitialValue * maximumAllowedPrepaymentPercentage);
        }

        public decimal GetBalance(DateTime at)
        {
            return GetMonthlyPayments(at)
                .Select(payment => payment.Balance)
                .Last();
        }

        public IEnumerable<MonthlyPayment> GetMonthlyPayments(DateTime at)
        {
            return Calculator.GetMonthlyPayments(this, at);
        }

        public IEnumerable<decimal> GetPrincipalOnlyPayments(int year, int month, int day)
        {
            var startAt = new DateTime(year, month, day);
            var endAt = startAt.AddDays(1);

            return GetPrepayments(startAt, endAt)
                .Select(payment => payment.Item2);
        }

        public decimal CostAt(int monthAfterInception)
        {
            if (monthAfterInception <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(monthAfterInception), "Should be greater than 0");
            }

            return Convert.ToDecimal(MonthlyPayment);
        }

        public decimal CostAt(DateTime at)
        {
            throw new NotImplementedException();
        }

        public decimal CostBy(int monthAfterInception)
        {
            if (monthAfterInception <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(monthAfterInception), "Should be greater than 0");
            }

            return Convert.ToDecimal(MonthlyPayment) * monthAfterInception;
        }

        public decimal CostBy(DateTime at)
        {
            throw new NotImplementedException();
        }

        public void AddPrepayment(DateTime at, decimal amount)
        {
            Prepayments.Add(at, amount);
        }

        public IEnumerable<ValueTuple<DateTime, decimal>> GetPrepayments(DateTime startAt, DateTime endAt)
        {
            return Prepayments.GetRange(startAt, endAt);
        }

        public bool IsMonthlyPayment(DateTime at)
        {
            return BaseMortgage.IsMonthlyPayment(at);
        }

        public void PrintPrepayments()
        {
            foreach (var prepayment in Prepayments.GetAll())
            {
                Console.WriteLine($"{prepayment.Item1}: {prepayment.Item2}");
            }
        }
    }
}
