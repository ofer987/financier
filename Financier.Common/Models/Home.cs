using System;
using System.Collections.Generic;
using System.Linq;

using Financier.Common.Calculations;
using Financier.Common.Models.Expenses;

namespace Financier.Common.Models
{
    public class Home : Product
    {
        public HomeValue Value => (HomeValue)Assets.First();

        public Home(decimal purchasePrice, IDictionary<string, decimal> expenses, decimal downPayment, DateTime purchasedAt, decimal interestRate, int amortisationPeriodInMonths = 300) : base(purchasedAt)
        {
            Assets.Add(new HomeValue(this, purchasePrice));

            Liabilities.Add(new MonthlyExpenses(this, expenses));
            Liabilities.Add(new Mortgage(this, downPayment, purchasePrice, interestRate, amortisationPeriodInMonths));
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
                return Value.PurchasePrice;
            }

            return Convert.ToDecimal(Math.Pow(Convert.ToDouble(MonthlyInflationRate), year - purchasedAtYear) * Convert.ToDouble(Value.PurchasePrice));
        }

        // public decimal GetTotalYearlyExpenses(int monthAfterInception)
        // {
        //     if (monthAfterInception < 0)
        //     {
        //         throw new Exception($"{nameof(monthAfterInception)} cannot be negative number");
        //     }
        //
        //     var result = 0.00M;
        //
        //     for (var i = monthAfterInception; i < monthAfterInception + 12; i += 1)
        //     {
        //         result += Expenses.MonthlyTotal + Mortgage.GetMonthlyInterestPayment(i);
        //     }
        //     return result;
        // }

        // public decimal CostAt(DateTime at)
        // {
        //     return CostAt(at.SubtractWholeMonths(PurchasedAt));
        // }
        //
        // public decimal CostAt(int monthAfterInception)
        // {
        //     if (monthAfterInception < 0)
        //     {
        //         throw new Exception($"{nameof(monthAfterInception)} cannot be negative number");
        //     }
        //
        //     return Expenses.MonthlyTotal + Mortgage.GetMonthlyInterestPayment(monthAfterInception);
        // }

        // public decimal CostBy(DateTime at)
        // {
        //     return CostBy(at.SubtractWholeMonths(PurchasedAt));
        // }
        //
        // public decimal CostBy(int monthAfterInception)
        // {
        //     if (monthAfterInception < 0)
        //     {
        //         throw new Exception($"{nameof(monthAfterInception)} cannot be negative number");
        //     }
        //
        //     var totalExpenses = monthAfterInception * Expenses.MonthlyTotal;
        //     var interestPayments = Mortgage.GetInterestPaymentsBy(monthAfterInception);
        //
        //     return Expenses.MonthlyTotal + interestPayments;
        // }
        //
        // // How much off the mortgage has been paid?
        // public decimal ValueAt(int monthAfterInception)
        // {
        //     if (monthAfterInception < 0)
        //     {
        //         throw new Exception($"{nameof(monthAfterInception)} cannot be negative number");
        //     }
        //
        //     var effectiveValuationInterestRateMonthly = Math.Pow(((Convert.ToDouble(YearlyValuationRate) / 100) + 1), 1.0/12) - 1;
        //
        //     return Convert.ToDecimal(Math.Pow(effectiveValuationInterestRateMonthly, monthAfterInception) * Convert.ToDouble(PurchasePrice));
        // }
        //
        // public decimal ValueBy(int monthAfterInception)
        // {
        //     return ValueAt(monthAfterInception);
        // }

        // The price of the home - expenses incurred
        // public decimal TotalBy(DateTime at)
        // {
        //     var months = at.SubtractWholeMonths(PurchasedAt);
        //     var effectiveValuationInterestRateMonthly = Math.Pow(((Convert.ToDouble(YearlyValuationRate) / 100) + 1), 1.0/12) - 1;
        //
        //     var priceAt = Convert.ToDecimal(Math.Pow(effectiveValuationInterestRateMonthly, months) * Convert.ToDouble(PurchasePrice));
        //     var expenses = CostBy(at);
        //     return priceAt - expenses;
        // }
    }
}
