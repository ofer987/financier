using System;

using Financier.Common.Models;
using Financier.Common.Liabilities;

namespace Financier.Common.Expenses.BalanceSheets
{
    public class RealEstateBuilder
    {
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

        public BalanceSheet AddHomeWithFixedRateMortgage(DateTime purchasedAt, Money purchasePrice)
        {
            var availableCash = Result.GetCashAt(purchasedAt);
            var downPayment = availableCash < purchasePrice
                ? availableCash
                : purchasePrice;
            var home = new Home("foobar", purchasedAt, downPayment, CreateFixedRateMortgage(purchasePrice.Value - downPayment, purchasedAt));

            Result.AddHome(home);
            Result.AddCashAdjustment(purchasedAt, new Money(downPayment, purchasedAt));

            return Result;
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
