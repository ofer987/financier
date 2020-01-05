using System;
using System.Collections.Generic;
using System.Linq;

namespace Financier.Common.Liabilities
{
    public class PrepayableMortgage : IMortgage, IPrepayable
    {
        public IMortgage BaseMortgage { get; }
        public CappedPayments Prepayments { get; }

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

        public PrepayableMortgage(IMortgage baseMortgage, decimal maximumAllowedPrepaymentPercentage = 0.10M)
        {
            BaseMortgage = baseMortgage;
            Prepayments = new CappedPayments(InitialValue * maximumAllowedPrepaymentPercentage);
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
            throw new NotImplementedException();

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

                var principalPayment = monthlyPayment - interestPayment + GetMonthlyPrepayment(at.Year, at.Month, at.Day);
                balance -= decimal.Round(Convert.ToDecimal(principalPayment), 2);

                yield return decimal.Round(interestPayment, 2);
            }
        }

        public IEnumerable<decimal> GetMonthlyInterestPayments()
        {
            throw new NotImplementedException();

            var monthlyPayment = Convert.ToDecimal(MonthlyPayment);
            var balance = InitialValue;
            var interestRate = BaseMortgage.PeriodicAnnualInterestRate;

            for (var i = 0; balance > 0; i += 1)
            {
                var at = InitiatedAt.AddMonths(i);
                var interestPayment = Convert.ToDecimal(Convert.ToDouble(balance) * interestRate / 12);

                var principalPayment = monthlyPayment - interestPayment + GetDailyPrepayment(at.Year, at.Month, at.Day);
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
            throw new NotImplementedException();

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
                var principalPayment = monthlyPayment - interestPayment + GetDailyPrepayment(at.Year, at.Month, at.Day);

                balance -= principalPayment;

                yield return decimal.Round(principalPayment, 2);
            }
        }

        public IEnumerable<decimal> GetMonthlyPrincipalPayments()
        {
            throw new NotImplementedException();

            var monthlyPayment = Convert.ToDecimal(MonthlyPayment);
            var balance = InitialValue;
            var interestRate = BaseMortgage.PeriodicAnnualInterestRate;

            for (var i = 0; balance > 0; i += 1)
            {
                var at = InitiatedAt.AddMonths(i);
                var interestPayment = Convert.ToDecimal(Convert.ToDouble(balance) * interestRate / 12);
                var principalPayment = monthlyPayment - interestPayment + GetDailyPrepayment(at.Year, at.Month, at.Day);

                balance -= principalPayment;

                yield return decimal.Round(principalPayment, 2);
            }
        }

        public decimal GetBalance(int monthAfterInception)
        {
            throw new NotImplementedException();

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
                var principalPayment = monthlyPayment - interestPayment + GetDailyPrepayment(at.Year, at.Month, at.Day);

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

            var i = InitiatedAt;
            for (; balance > 0 && i < at; i = i.AddDays(1))
            {
                if (IsMonthlyPayment(i))
                {
                    var interestPayment = Convert.ToDecimal(Convert.ToDouble(balance) * interestRate / 12);
                    var principalPayment = monthlyPayment - interestPayment;

                    if (balance - principalPayment < 0)
                    {
                        // balance -= principalPayment;
                        Console.WriteLine($"To me: {balance - principalPayment}");
                        return 0.00M;
                    }
                    else
                    {
                        balance -= principalPayment;
                    }
                }
                
                balance -= GetDailyPrepayment(i.Year, i.Month, i.Day);
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

        public void PrintPrepayments()
        {
            foreach (var prepayment in Prepayments.GetAll())
            {
                Console.WriteLine($"{prepayment.Item1}: {prepayment.Item2}");
            }
        }

        public decimal GetMonthlyPrepayment(int year, int month, int day = 1)
        {
            var endAt = new DateTime(year, month, day);
            var startAt = endAt.AddMonths(-1);

            // Console.WriteLine($"Get prepayments: {startAt} <= at < {endAt}");
            return Prepayments.GetRange(startAt, endAt)
                .Select(payment => payment.Item2)
                .Sum();
        }

        public decimal GetDailyPrepayment(int year, int month, int day)
        {
            var startAt = new DateTime(year, month, day);
            var endAt = startAt.AddDays(1);

            // Console.WriteLine($"Get prepayments: {startAt} <= at < {endAt}");
            return Prepayments.GetRange(startAt, endAt)
                .Select(payment => payment.Item2)
                .Sum();
        }

        public IEnumerable<ValueTuple<DateTime, decimal>> GetPrepayments(DateTime startAt, DateTime endAt)
        {
            return Prepayments.GetRange(startAt, endAt);
        }

        private bool IsMonthlyPayment(DateTime at)
        {
            return at.Day == InitiatedAt.Day;
        }
    }
}
