using System;
using System.Collections.Generic;

using Financier.Common.Calculations;
using Financier.Common.Models.Expenses;

namespace Financier.Common.Models
{
    public class Home : Product
    {
        public HomeValue Value { get; }

        public MonthlyExpenses Expenses { get; }

        public Mortgage Mortgage { get; }

        public decimal TaxesByMonth => Expenses.Values["taxes"];

        public Home(string name, decimal purchasePrice, IDictionary<string, decimal> expenses, decimal downPayment, DateTime purchasedAt, decimal interestRate, int amortisationPeriodInMonths = 300) : base(name, purchasedAt)
        {
            Assets.Add(Value = new HomeValue(this, purchasePrice));
            Liabilities.Add(Expenses = new MonthlyExpenses(this, expenses));
            Liabilities.Add(Mortgage = new Mortgage(this, downPayment, purchasePrice, interestRate, amortisationPeriodInMonths));
        }

        public Home(Home source, string name, decimal downPayment, DateTime purchasedAt, decimal interestRate, int amortisationPeriodInMonths = 300) : base(name, purchasedAt)
        {
            var currentPurchasePrice = source.GetPriceAtYear(purchasedAt.Year);

            Assets.Add(Value = new HomeValue(this, currentPurchasePrice));
            Liabilities.Add(Expenses = new MonthlyExpenses(this, source.Expenses.Values));
            Liabilities.Add(Mortgage = new Mortgage(this, downPayment, currentPurchasePrice, interestRate, amortisationPeriodInMonths));
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

            return Convert.ToDecimal(Math.Pow(Convert.ToDouble(MonthlyValuationRate), year - purchasedAtYear) * Convert.ToDouble(Value.PurchasePrice));
        }
    }
}
