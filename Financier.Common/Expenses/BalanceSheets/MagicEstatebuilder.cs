using System;

using Financier.Common.Models;
using Financier.Common.Liabilities;
using Financier.Common.Expenses.Actions;
using Financier.Common.Extensions;

namespace Financier.Common.Expenses.BalanceSheets
{
    public class MagicEstateBuilder
    {
        private DateTime At;
        private Activity Result { get; set; }

        public MagicEstateBuilder(ICashFlow cashFlow, DateTime at)
        {
            Result = new Activity(cashFlow, at);
            At = at;
        }

        public MagicEstateBuilder SetInitialCash(Money cash)
        {
            Result.InitialCash = cash;

            return this;
        }

        public MagicEstateBuilder SetInitialDebt(Money debt)
        {
            Result.InitialDebt = debt;

            return this;
        }

        public MagicEstateBuilder AddCondoWithFixedRateMortgageAtDownPaymentPercentage(decimal purchasePriceAtInitiation, decimal downPaymentPercentage, decimal interestRate = 0.0319M)
        {
            if (purchasePriceAtInitiation < 0.00M)
            {
                throw new ArgumentOutOfRangeException(nameof(purchasePriceAtInitiation), purchasePriceAtInitiation, "Should be at or greater than 0.00");
            }

            if (downPaymentPercentage < 0.00M || downPaymentPercentage > 100.00M)
            {
                throw new ArgumentOutOfRangeException(nameof(downPaymentPercentage), downPaymentPercentage, "Should be between 0.00 and 100.00 per cent (inclusive)");
            }

            // Baseline for prices
            var initiatedAt = At;

            // Does not take into account inflation
            Func<DateTime, decimal> downPaymentAmountFunc = (at) => 
            {
                return new Money(
                    downPaymentPercentage * purchasePriceAtInitiation,
                    initiatedAt
                ).GetValueAt(Inflations.GetInflation(InflationTypes.CompoundYearlyInflation, 0.05M), at).Value;
            };

            var purchasedAt = HasCashAt(downPaymentAmountFunc, initiatedAt);
            var downPaymentAmount = new Money(
                downPaymentPercentage * purchasePriceAtInitiation,
                initiatedAt
            ).GetValueAt(Inflations.GetInflation(InflationTypes.CompoundYearlyInflation, 0.05M), purchasedAt);
            var mortgageAmount = new Money(
                (100 - downPaymentPercentage) * purchasePriceAtInitiation,
                initiatedAt
            ).GetValueAt(Inflations.GetInflation(InflationTypes.CompoundYearlyInflation, 0.05M), purchasedAt);
            var fullAmount = new Money(
                purchasePriceAtInitiation,
                initiatedAt
            ).GetValueAt(Inflations.GetInflation(InflationTypes.CompoundYearlyInflation, 0.05M), purchasedAt);
            var mortgage = Mortgages.GetFixedRateMortgage(
                mortgageAmount.Reverse,
                0.0319M,
                300,
                purchasedAt,
                downPaymentAmount
            );
            var home = new Home(
                "Condo",
                purchasedAt,
                fullAmount,
                downPaymentAmount,
                mortgage
            );

            Result.Buy(mortgage, purchasedAt);
            Result.Buy(home, purchasedAt);

            At = purchasedAt.GetNext();

            return this;
        }

        public MagicEstateBuilder SellHome(Home home, DateTime soldAt, Money soldPrice)
        {
            // TODO: implement me!
            return this;
        }

        public Activity Build()
        {
            return Result;
        }

        public DateTime HasCashAt(Func<DateTime, decimal> expected, DateTime startAt)
        {
            var inflation = Inflations.GetInflation(InflationTypes.NoopInflation);
            for (var i = startAt; true; i = i.GetNext())
            {
                if (Result.GetCash(inflation, i) >= expected(i))
                {
                    return i;
                }
            }
        }
    }
}
