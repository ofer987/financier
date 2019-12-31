using System;
using System.Collections.Generic;
using System.Linq;

namespace Financier.Common.Liabilities
{
    public class PrepayableMortgage : IMortgage, IPrepayable
    {
        public IMortgage BaseMortgage { get; }
        public Prepayments Prepayments { get; }

        public decimal BaseValue => BaseMortgage.BaseValue;
        public decimal InitialValue => BaseMortgage.InitialValue;
        public DateTime InitiatedAt => BaseMortgage.InitiatedAt;

        public int AmortisationPeriodInMonths => BaseMortgage.AmortisationPeriodInMonths;
        public decimal InterestRate => BaseMortgage.InterestRate;
        public decimal QuotedInterestRate => BaseMortgage.QuotedInterestRate;

        public double PeriodicMonthlyInterestRate => BaseMortgage.PeriodicMonthlyInterestRate;
        public double PeriodicAnnualInterestRate => BaseMortgage.PeriodicAnnualInterestRate;
        public double EffectiveAnnualInterestRate => BaseMortgage.EffectiveAnnualInterestRate;

        public decimal MaximumAllowedPrepaymentTotal => Prepayments.MaximumAllowedAnnualTotal;
        public double MonthlyPayment => BaseMortgage.MonthlyPayment;

        public PrepayableMortgage(IMortgage baseMortgage, DateTime initiatedAt, decimal maximumAllowedPrepaymentPercentage = 0.10M)
        {
            BaseMortgage = baseMortgage;
            Prepayments = new Prepayments(InitialValue, maximumAllowedPrepaymentPercentage);
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

        public IEnumerable<decimal> GetMonthlyInterestPayments(int monthAfterInception)
        {
            if (monthAfterInception <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(monthAfterInception), "Should be greater than 0");
            }

            var monthlyPayment = Convert.ToDecimal(MonthlyPayment);
            var balance = InitialValue;
            var interestRate = BaseMortgage.PeriodicAnnualInterestRate;

            for (var i = 0; balance > 0 && i < monthAfterInception; i += 1)
            {
                var at = InitiatedAt.AddMonths(i);
                var interestPayment = Convert.ToDecimal(Convert.ToDouble(balance) * interestRate / 12);

                var principalPayment = monthlyPayment - interestPayment + GetPrepayment(at.Year, at.Month);
                balance -= decimal.Round(Convert.ToDecimal(principalPayment), 2);

                yield return decimal.Round(interestPayment, 2);
            }
        }

        public IEnumerable<decimal> GetMonthlyInterestPayments()
        {
            var monthlyPayment = Convert.ToDecimal(MonthlyPayment);
            var balance = InitialValue;
            var interestRate = BaseMortgage.PeriodicAnnualInterestRate;

            for (var i = 0; balance > 0; i += 1)
            {
                var at = InitiatedAt.AddMonths(i);
                var interestPayment = Convert.ToDecimal(Convert.ToDouble(balance) * interestRate / 12);

                var principalPayment = monthlyPayment - interestPayment + GetPrepayment(at.Year, at.Month);
                balance -= decimal.Round(Convert.ToDecimal(principalPayment), 2);

                yield return decimal.Round(interestPayment, 2);
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

        public IEnumerable<decimal> GetMonthlyPrincipalPayments(int monthAfterInception)
        {
            if (monthAfterInception < 0)
            {
                throw new Exception($"{nameof(monthAfterInception)} cannot be negative number");
            }

            var monthlyPayment = Convert.ToDecimal(MonthlyPayment);
            var balance = InitialValue;
            var interestRate = BaseMortgage.PeriodicAnnualInterestRate;

            for (var i = 0; balance > 0 && i < monthAfterInception; i += 1)
            {
                var at = InitiatedAt.AddMonths(i);
                var interestPayment = Convert.ToDecimal(Convert.ToDouble(balance) * interestRate / 12);
                var principalPayment = monthlyPayment - interestPayment + GetPrepayment(at.Year, at.Month);

                balance -= principalPayment;

                yield return decimal.Round(principalPayment, 2);
            }
        }

        public IEnumerable<decimal> GetMonthlyPrincipalPayments()
        {
            var monthlyPayment = Convert.ToDecimal(MonthlyPayment);
            var balance = InitialValue;
            var interestRate = BaseMortgage.PeriodicAnnualInterestRate;

            for (var i = 0; balance > 0; i += 1)
            {
                var at = InitiatedAt.AddMonths(i);
                var interestPayment = Convert.ToDecimal(Convert.ToDouble(balance) * interestRate / 12);
                var principalPayment = monthlyPayment - interestPayment + GetPrepayment(at.Year, at.Month);

                balance -= principalPayment;

                yield return decimal.Round(principalPayment, 2);
            }
        }

        public decimal GetBalance(int monthAfterInception)
        {
            if (monthAfterInception < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(monthAfterInception), "Should be equal or greater than 0");
            }

            var monthlyPayment = Convert.ToDecimal(MonthlyPayment);
            var balance = InitialValue;
            var interestRate = BaseMortgage.PeriodicAnnualInterestRate;

            for (var i = 0; balance > 0 && i < monthAfterInception; i += 1)
            {
                var at = InitiatedAt.AddMonths(i);
                var interestPayment = Convert.ToDecimal(Convert.ToDouble(balance) * interestRate / 12);
                var principalPayment = monthlyPayment - interestPayment + GetPrepayment(at.Year, at.Month);

                balance -= principalPayment;
            }

            return decimal.Round(balance, 2);
        }

        public decimal GetBalance(DateTime at)
        {
            if (at < InitiatedAt)
            {
                throw new ArgumentOutOfRangeException(nameof(at), $"Should be at or later than {InitiatedAt}");
            }

            var monthlyPayment = Convert.ToDecimal(MonthlyPayment);
            var balance = InitialValue;
            var interestRate = BaseMortgage.PeriodicAnnualInterestRate;

            for (var i = InitiatedAt; balance > 0 && i < at; i = i.AddMonths(1))
            {
                var interestPayment = Convert.ToDecimal(Convert.ToDouble(balance) * interestRate / 12);
                var principalPayment = monthlyPayment - interestPayment + GetPrepayment(i.Year, i.Month);

                balance -= principalPayment;
            }

            return decimal.Round(balance, 2);
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

        public decimal GetPrepayment(int year, int month)
        {
            return Prepayments.GetMonthlyTotal(year, month);
        }

        public IEnumerable<ValueTuple<DateTime, decimal>> GetPrepayments(DateTime startAt, DateTime endAt)
        {
            return Prepayments.GetRange(startAt, endAt);
        }
    }
}
