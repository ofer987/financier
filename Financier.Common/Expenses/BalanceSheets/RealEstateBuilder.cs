using System;

using Financier.Common.Models;
using Financier.Common.Liabilities;
using Financier.Common.Expenses.Actions;

namespace Financier.Common.Expenses.BalanceSheets
{
    public class RealEstateBuilder
    {
        private DateTime InitiatedAt => Result.InitiatedAt;
        private Activity Result { get; set; }

        public RealEstateBuilder(ICashFlow cashFlow, DateTime at)
        {
            Result = new Activity(cashFlow, at);
        }

        public Activity SetInitialCash(Money cash)
        {
            Result.InitialCash = cash;

            return Result;
        }

        public Activity SetInitialDebt(Money debt)
        {
            Result.InitialDebt = debt;

            return Result;
        }

        public RealEstateBuilder AddCondoWithFixedRateMortgage(DateTime purchasedAt, decimal purchasePriceAtInitiation)
        {
            return AddHomeWithFixedRateMortgage(
                purchasedAt,
                purchasePriceAtInitiation,
                new CompoundYearlyInflation(0.05M)
            );
        }

        public RealEstateBuilder AddCondoTownhouseWithFixedRateMortgage(DateTime purchasedAt, decimal purchasePriceAtInitiation)
        {
            return AddHomeWithFixedRateMortgage(
                purchasedAt,
                purchasePriceAtInitiation,
                new CompoundYearlyInflation(0.08M)
            );
        }

        public RealEstateBuilder AddFreeholdWithFixedRateMortgage(DateTime purchasedAt, decimal purchasePriceAtInitiation)
        {
            return AddHomeWithFixedRateMortgage(
                purchasedAt,
                purchasePriceAtInitiation,
                new CompoundYearlyInflation(0.10M)
            );
        }

        public RealEstateBuilder SellHome(Home home, DateTime soldAt, Money soldPrice)
        {
            // TODO: implement me!
            return this;
        }

        public Activity Build()
        {
            return Result;
        }

        private RealEstateBuilder AddHomeWithFixedRateMortgage(DateTime purchasedAt, decimal purchasePriceAtInitiation, IInflation inflation)
        {
            var purchasePriceWhenPurchased = new Money(purchasePriceAtInitiation, InitiatedAt)
                .GetValueAt(new CompoundYearlyInflation(0.05M), purchasedAt);
            var availableCash = Result.GetCash(inflation, purchasedAt);
            var downPaymentAmount = availableCash < purchasePriceAtInitiation
                ? availableCash
                : purchasePriceAtInitiation;
            var mortgage = Mortgages.GetFixedRateMortgage(
                new Money(purchasePriceWhenPurchased.Value - downPaymentAmount, purchasedAt),
                0.0319M,
                300,
                purchasedAt,
                new Money(downPaymentAmount, purchasedAt)
            );

            var home = new Home(
                "foobar",
                purchasedAt,
                new Money(purchasePriceWhenPurchased, purchasedAt),
                new Money(downPaymentAmount, purchasedAt),
                mortgage
            );

            Result.Buy(home, purchasedAt);
            Result.Buy(mortgage, purchasedAt);

            return this;
        }
    }
}
