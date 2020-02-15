using System;

using Financier.Common.Models;
using Financier.Common.Liabilities;

namespace Financier.Common.Expenses.BalanceSheets
{
    public class RealEstateBuilder
    {
        private DateTime InitiatedAt => Result.InitiatedAt;
        private BalanceSheet Result { get; set; }

        public RealEstateBuilder(ICashFlow cashFlow, DateTime at)
        {
            Result = new BalanceSheet(cashFlow, at);
        }

        public BalanceSheet SetInitialCash(Money cash)
        {
            Result.InitialCash = cash;

            return Result;
        }

        public BalanceSheet SetInitialDebt(Money debt)
        {
            Result.InitialDebt = debt;

            return Result;
        }

        public RealEstateBuilder AddHomeWithFixedRateMortgage(DateTime purchasedAt, decimal purchasePriceAtInitiation)
        {
            var purchasePriceWhenPurchased = new Money(purchasePriceAtInitiation, InitiatedAt)
                .GetValueAt(new CompoundYearlyInflation(0.05M), purchasedAt);
            var availableCash = Result.GetCashAt(purchasedAt);
            var downPaymentAmount = availableCash < purchasePriceAtInitiation
                ? availableCash
                : purchasePriceAtInitiation;
            var mortgage = CreateFixedRateMortgage(
                purchasePriceWhenPurchased.Value - downPaymentAmount,
                purchasedAt
            );
            

            var home = new Home(
                "foobar",
                purchasedAt,
                new Money(purchasePriceWhenPurchased, purchasedAt),
                new Money(downPaymentAmount, purchasedAt),
                mortgage
            );

            Result.AddHome(home);
            Result.AddCashAdjustment(purchasedAt, new Money(downPaymentAmount, purchasedAt));

            return this;
        }

        private IMortgage CreateFixedRateMortgage(decimal amount, DateTime at)
        {
            return new FixedRateMortgage(new Money(amount, at), 0.0319M, 300, at);
        }

        public BalanceSheet Build()
        {
            return Result;
        }
    }
}
