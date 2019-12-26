using System;
using System.Collections.Generic;
using System.Linq;

namespace Financier.Common.Liabilities
{
    public class PrepayableMortgage : IMortgage
    {
        public Mortgage BaseMortgage { get; }
        public DateTime InitiatedAt { get; }

        public decimal MaximumAllowedPrepaymentTotal { get; }
        private decimal DefaultMaximumAllowedPrepaymentTotal => decimal.Round(BaseMortgage.InitialValue / 10.00M, 2);
        public double MonthlyPayment => BaseMortgage.MonthlyPayment;

        private IDictionary<DateTime, decimal> prepayments = new Dictionary<DateTime, decimal>();
        public IReadOnlyDictionary<DateTime, decimal> Prepayments => (IReadOnlyDictionary<DateTime, decimal>)prepayments;

        public PrepayableMortgage(Mortgage baseMortgage, DateTime initiatedAt, decimal maximumAllowedPrepaymentPercentage = 0.10M)
        {
            BaseMortgage = baseMortgage;
            InitiatedAt = initiatedAt;
            MaximumAllowedPrepaymentTotal = decimal.Round(BaseMortgage.InitialValue * maximumAllowedPrepaymentPercentage, 2);
        }

        public void AddPrepayment(DateTime at, decimal amount)
        {
            var annualTotal = GetAnnualPrepaidTotal(at.Year);

            if (annualTotal >= MaximumAllowedPrepaymentTotal)
            {
                throw new Exception($"Exceeded total prepayment amount ({MaximumAllowedPrepaymentTotal})");
            }

            prepayments.Add(at, amount);
        }

        public decimal GetAnnualPrepaidTotal(int year)
        {
            var beginningOfYear = new DateTime(year, 1, 1);
            var beginningOfNextYear = new DateTime(year + 1, 1, 1);

            return prepayments
                .Where(prepayment => prepayment.Key >= beginningOfYear)
                .Where(prepayment => prepayment.Key < beginningOfNextYear)
                .Select(prepayment => prepayment.Value)
                .Sum();
        }

        public decimal GetPrepayment(int year, int month)
        {
            var beginningOfMonth = new DateTime(year, month, 1);
            var beginningOfNextMonth = new DateTime(year, month, 1).AddMonths(1);

            return prepayments
                .Where(prepayment => prepayment.Key >= beginningOfMonth)
                .Where(prepayment => prepayment.Key < beginningOfNextMonth)
                .Select(prepayment => prepayment.Value)
                .Sum();
        }

        public IEnumerable<decimal> GetMonthlyInterestPayments(int monthAfterInception)
        {
            if (monthAfterInception <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(monthAfterInception), "Should be greater than 0");
            }

            var monthlyPayment = Convert.ToDecimal(MonthlyPayment);
            var balance = BaseMortgage.InitialValue;
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
            var balance = BaseMortgage.InitialValue;
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

        public IEnumerable<decimal> GetMonthlyPrincipalPayments(int monthAfterInception)
        {
            if (monthAfterInception < 0)
            {
                throw new Exception($"{nameof(monthAfterInception)} cannot be negative number");
            }

            var monthlyPayment = Convert.ToDecimal(MonthlyPayment);
            var balance = BaseMortgage.InitialValue;
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
            var balance = BaseMortgage.InitialValue;
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

        public decimal GetBalance(int monthAfterInception)
        {
            if (monthAfterInception <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(monthAfterInception), "Should be greater than 0");
            }

            var monthlyPayment = Convert.ToDecimal(MonthlyPayment);
            var balance = BaseMortgage.InitialValue;
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

        public decimal CostAt(int monthAfterInception)
        {
            throw new NotImplementedException();
        }

        public decimal CostAt(DateTime at)
        {
            throw new NotImplementedException();
        }

        public decimal CostBy(int monthAfterInception)
        {
            throw new NotImplementedException();
        }

        public decimal CostBy(DateTime at)
        {
            throw new NotImplementedException();
        }
    }
}
