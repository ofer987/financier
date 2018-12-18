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

        private DateTime? soldAt = null;
        public DateTime SoldAt
        {
            get
            {
                if (!soldAt.HasValue)
                {
                    throw new Exception("Product has not been sold yet");
                }

                return soldAt.Value;
            }

            set
            {
                if (value < PurchasedAt)
                {
                    throw new Exception($"Product (insert identifier) cannot be sold at ({value}) before it was purchased at ({PurchasedAt})");
                }

                SoldAt = value;
            }
        }

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
            return CostAt(at.SubtractWholeMonths(PurchasedAt));
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
            return CostBy(at.SubtractWholeMonths(PurchasedAt));
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
            return ValueAt(at.SubtractWholeMonths(PurchasedAt));
        }

        public decimal ValueAt(int monthAfterInception)
        {
            if (monthAfterInception < 0)
            {
                throw new Exception($"{nameof(monthAfterInception)} cannot be negative number");
            }

            var effectiveValuationInterestRateMonthly = Math.Pow(((Convert.ToDouble(YearlyValuationRate) / 100) + 1), 1.0/12) - 1;

            return Convert.ToDecimal(Math.Pow(effectiveValuationInterestRateMonthly, monthAfterInception) * Convert.ToDouble(PurchasePrice));
        }

        public decimal ValueBy(DateTime at)
        {
            return ValueBy(at.SubtractWholeMonths(PurchasedAt));
        }

        public decimal ValueBy(int monthAfterInception)
        {
            return ValueAt(monthAfterInception);
        }

        public decimal TotalBy(DateTime at)
        {
            var months = at.SubtractWholeMonths(PurchasedAt);
            var effectiveValuationInterestRateMonthly = Math.Pow(((Convert.ToDouble(YearlyValuationRate) / 100) + 1), 1.0/12) - 1;

            var priceAt = Convert.ToDecimal(Math.Pow(effectiveValuationInterestRateMonthly, months) * Convert.ToDouble(PurchasePrice));
            var expenses = CostBy(at);
            return priceAt - expenses;
        }

        public decimal Sell(DateTime soldAt)
        {
            return TotalBy(soldAt);
        }
    }
}
