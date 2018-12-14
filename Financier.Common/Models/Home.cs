using System;

using Financier.Common.Calculations;
using Financier.Common.Models.Expenses;

namespace Financier.Common.Models
{
    public class Home : Asset
    {
        // TODO: place in configuration file or should be configurable somehow
        public const decimal YearlyInflationRate = 2.00M;

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
            var result = 0.00M;

            for (var i = monthAfterInception; i < monthAfterInception + 12; i += 1)
            {
                result += Expenses.MonthlyTotal + Mortgage.GetMonthlyInterestPayment(i);
            }
            return result;
        }
    }
}
