using System;

using Financier.Common.Models;
using Financier.Common.Extensions;

namespace Financier.Common.Calculations
{
    public class WhenToBuy
    {
        // TODO: place in configuration file or should be configurable somehow
        public const decimal YearlyInflationRate = 2.00M;

        // TODO: place in configuration file or should be configurable somehow
        public const decimal YearlyValuationRate = 5.00M;

        // TODO: place in configuration file or should be configurable somehow
        public const decimal YearlyMortgageRate = 3.19M;

        public DateTime AHome(Person person, DateTime from, DateTime to, Home desiredHome)
        {
            var highestValue = Decimal.MinValue;
            var highestValueAt = from;
            for (var i = 0; i < to.SubtractWholeMonths(from); i += 1)
            {
                var purchasedAt = from.AddMonths(i);
                var expenses = HomeValue(person, from, to, desiredHome, purchasedAt);
                if (expenses > highestValue)
                {
                    highestValue = expenses;
                    highestValueAt = purchasedAt;
                }
            }

            return highestValueAt;
        }

        public decimal HomeValue(Person person, DateTime from, DateTime to, Home desiredHome, DateTime purchasedAt)
        {
            // Home Value: desiredHome.GetPriceAtYear() // This is a constant
            // So the thing that changes is the expenses
            //
            // Expenses:
            //  Taxes: currentTaxes * Inflation(to.Year, purchasedAt)
            //  Mortgage Payment = GetMonthlyMortgagePayment(purchasedAt - DateTime.Now) * (to.Year - purchasedAt)
            //  Mortgage payments = Mortgage Payments * (to.Year - purchasedAt)
            var monthlyValuationRate = GetEffectiveMonthlyRate(Convert.ToDouble(YearlyValuationRate));
            var monthlyInterestRate = GetEffectiveMonthlyRate(Convert.ToDouble(YearlyInflationRate));
            var monthlyMorgageRate = GetEffectiveMonthlyRate(Convert.ToDouble(YearlyMortgageRate));

            var priceAt = PriceAt(desiredHome.PurchasePrice, monthlyValuationRate, from, purchasedAt);
            var monthlyMortgagePayment = GetMonthlyMortgagePayment(priceAt, monthlyMorgageRate);
            var mortgagePayaments = GetMortgagePayments(monthlyMortgagePayment, purchasedAt, to);

            var taxes = GetTotalExpenses(desiredHome.TaxesByMonth, monthlyInterestRate, to.SubtractWholeMonths(purchasedAt));

            var total = 0 - priceAt - mortgagePayaments - taxes;

            return total;
        }

        public decimal GetMonthlyMortgagePayment(decimal purchasePrice, double effectiveInterestRateMonthly, int amortisationPeriodInMonths = 300)
        {
            return Convert.ToDecimal(Convert.ToDouble(purchasePrice) * effectiveInterestRateMonthly / ( 1 - Math.Pow((1 + effectiveInterestRateMonthly), -1 * amortisationPeriodInMonths)));
        }

        public decimal GetMortgagePayments(decimal monthlyMortgagePayment, DateTime purchasedAt, DateTime to)
        {
            return monthlyMortgagePayment * to.SubtractWholeMonths(purchasedAt);
        }

        public decimal PriceAt(decimal purchasePrice, double effectiveMonthlyValuationRate, DateTime from, DateTime purchasedAt)
        {
            return Convert.ToDecimal(Convert.ToDouble(purchasePrice) * Math.Pow(effectiveMonthlyValuationRate, purchasedAt.SubtractWholeMonths(from)));
        }

        public decimal GetMonthlyExpense(decimal priceAtInceptionMonth, double interest, int monthAfterInception)
        {
            return Convert.ToDecimal(Math.Pow(interest, monthAfterInception) * Convert.ToDouble(priceAtInceptionMonth));
        }

        public decimal GetTotalExpenses(decimal priceAtInceptionMonth, double interest, int totalMonths)
        {
            var result = 0.00M;
            for (var i = 0; i < totalMonths; i += 1)
            {
                result += GetMonthlyExpense(priceAtInceptionMonth, interest, i);
            }

            return result;
        }

        public double GetEffectiveMonthlyRate(double quotedYearlyRate)
        {
            return Math.Pow(Math.Pow((Convert.ToDouble(quotedYearlyRate) / (1 * 100) + 1), 1), 1.0/12) - 1;
        }
    }
}
