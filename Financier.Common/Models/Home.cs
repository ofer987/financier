using System;

using Financier.Common.Calculations;
using Financier.Common.Models.Expenses;
using Financier.Common.Extensions;

namespace Financier.Common.Models
{
    public class Home : IAsset, ILiability
    {
        // TODO: place in configuration file or should be configurable somehow
        public const decimal YearlyInflationRate = 2.00M;

        // TODO place in a configuration file or pass in as constructor parameter
        public const decimal YearlyValuationRate = 5.00M;

        public double MonthlyInflationRate => Math.Pow(Convert.ToDouble(YearlyInflationRate) / 100, 1.0/12) - 1;

        public decimal PurchasePrice { get; }

        public DateTime PurchasedAt { get; }

        public MonthlyExpenses Expenses { get; }

        public Mortgage Mortgage { get; }

        public Home(decimal purchasePrice, DateTime purchasedAt, Mortgage mortgage, MonthlyExpenses expenses)
        {
            PurchasePrice = purchasePrice;
            PurchasedAt = purchasedAt;
            Mortgage = mortgage;
            Expenses = expenses;
        }

        public decimal GetPriceAtYear(int year)
        {
            var purchasedAtYear = PurchasedAt.Year;
            if (year < purchasedAtYear)
            {
                throw new Exception($"The requested year ({year}) cannot be before the purchase year ({PurchasedAt})");
            }

            if (year == purchasedAtYear)
            {
                return PurchasePrice;
            }

            return Convert.ToDecimal(Math.Pow(Convert.ToDouble(MonthlyInflationRate), year - purchasedAtYear) * Convert.ToDouble(PurchasePrice));
        }

        public decimal GetTotalYearlyExpenses(int monthAfterInception)
        {
            if (monthAfterInception < 0)
            {
                throw new Exception($"{nameof(monthAfterInception)} cannot be negative number");
            }

            var result = 0.00M;

            for (var i = monthAfterInception; i < monthAfterInception + 12; i += 1)
            {
                result += Expenses.MonthlyTotal + Mortgage.GetMonthlyInterestPayment(i);
            }
            return result;
        }

        public decimal CostAt(DateTime at)
        {
            return CostAt(at.MonthDifference(PurchasedAt));
        }

        public decimal CostAt(int monthAfterInception)
        {
            if (monthAfterInception < 0)
            {
                throw new Exception($"{nameof(monthAfterInception)} cannot be negative number");
            }

            return Expenses.MonthlyTotal + Mortgage.GetMonthlyInterestPayment(monthAfterInception);
        }

        public decimal CostBy(DateTime at)
        {
            return CostBy(at.MonthDifference(PurchasedAt));
        }

        public decimal CostBy(int monthAfterInception)
        {
            if (monthAfterInception < 0)
            {
                throw new Exception($"{nameof(monthAfterInception)} cannot be negative number");
            }

            var totalExpenses = monthAfterInception * Expenses.MonthlyTotal;
            var interestPayments = Mortgage.GetInterestPaymentsBy(monthAfterInception);

            return Expenses.MonthlyTotal + interestPayments;
        }

        public decimal ValueAt(DateTime at)
        {
            return ValueAt(PurchasedAt.MonthDifference(at));
        }

        public decimal ValueAt(int monthAfterInception)
        {
            if (monthAfterInception < 0)
            {
                throw new Exception($"{nameof(monthAfterInception)} cannot be negative number");
            }

            var effectiveInterestRateMonthly = Math.Pow(((Convert.ToDouble(YearlyValuationRate) / 100) + 1), 1.0/12) - 1;

            return Convert.ToDecimal(Math.Pow(effectiveInterestRateMonthly, monthAfterInception) * Convert.ToDouble(PurchasePrice));
        }

        public decimal ValueBy(DateTime at)
        {
            return ValueBy(PurchasedAt.MonthDifference(at));
        }

        public decimal ValueBy(int monthAfterInception)
        {
            return ValueAt(monthAfterInception);
        }
    }
}
