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
    }
}
