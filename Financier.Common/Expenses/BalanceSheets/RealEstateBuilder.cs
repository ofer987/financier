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

        public RealEstateBuilder SetInitialCash(Money cash)
        {
            Result.InitialCash = cash;

            return this;
        }

        public RealEstateBuilder SetInitialDebt(Money debt)
        {
            Result.InitialDebt = debt;

            return this;
        }

        public RealEstateBuilder AddCondoWithFixedRateMortgage(DateTime purchasedAt, decimal purchasePriceAtInitiation)
        {
            return AddHomeWithFixedRateMortgage(
                purchasedAt,
                purchasePriceAtInitiation,
                Inflations.CondoPriceIndex
            );
        }

        public RealEstateBuilder AddCondoWithFixedRateMortgage(DateTime purchasedAt, Money purchasePrice)
        {
            return AddHomeWithFixedRateMortgage(
                purchasedAt,
                purchasePrice,
                Inflations.CondoPriceIndex
            );
        }

        public RealEstateBuilder AddCondoTownhouseWithFixedRateMortgage(DateTime purchasedAt, decimal purchasePriceAtInitiation)
        {
            return AddHomeWithFixedRateMortgage(
                purchasedAt,
                purchasePriceAtInitiation,
                Inflations.TownHousePriceIndex
            );
        }

        public RealEstateBuilder AddCondoTownhouseWithFixedRateMortgage(DateTime purchasedAt, Money purchasePrice)
        {
            return AddHomeWithFixedRateMortgage(
                purchasedAt,
                purchasePrice,
                Inflations.TownHousePriceIndex
            );
        }

        public RealEstateBuilder AddFreeholdWithFixedRateMortgage(DateTime purchasedAt, decimal purchasePriceAtInitiation)
        {
            return AddHomeWithFixedRateMortgage(
                purchasedAt,
                purchasePriceAtInitiation,
                Inflations.FreeHoldPriceIndex
            );
        }

        public RealEstateBuilder AddFreeholdWithFixedRateMortgage(DateTime purchasedAt, Money purchasePrice)
        {
            return AddHomeWithFixedRateMortgage(
                purchasedAt,
                purchasePrice,
                Inflations.FreeHoldPriceIndex
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
