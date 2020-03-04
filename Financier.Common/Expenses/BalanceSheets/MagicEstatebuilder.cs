using System;

using Financier.Common.Models;
using Financier.Common.Liabilities;
using Financier.Common.Expenses.Actions;
using Financier.Common.Extensions;

namespace Financier.Common.Expenses.BalanceSheets
{
    public class MagicEstateBuilder
    {
        private DateTime InitiatedAt => Result.InitiatedAt;
        private DateTime At;
        private Activity Result { get; set; }

        public MagicEstateBuilder(ICashFlow cashFlow, DateTime at)
        {
            Result = new Activity(cashFlow, at);
            At = InitiatedAt;
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

            // Does not take into account inflation
            Func<DateTime, decimal> downPaymentAmountFunc = (at) => 
            {
                return new Money(
                    downPaymentPercentage * purchasePriceAtInitiation,
                    InitiatedAt
                ).GetValueAt(Inflations.GetInflation(InflationTypes.CompoundYearlyInflation, 0.05M), at).Value;
            };

            var purchasedAt = HasCashAt(downPaymentAmountFunc);
            var downPaymentAmount = new Money(
                downPaymentPercentage * purchasePriceAtInitiation,
                InitiatedAt
            ).GetValueAt(Inflations.GetInflation(InflationTypes.CompoundYearlyInflation, 0.05M), purchasedAt);
            var mortgageAmount = new Money(
                (100 - downPaymentPercentage) * purchasePriceAtInitiation,
                InitiatedAt
            ).GetValueAt(Inflations.GetInflation(InflationTypes.CompoundYearlyInflation, 0.05M), purchasedAt);
            var fullAmount = new Money(
                purchasePriceAtInitiation,
                InitiatedAt
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

            At = At.GetNext();

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

        public DateTime HasCashAt(Func<DateTime, decimal> expected)
        {
            var inflation = Inflations.GetInflation(InflationTypes.NoopInflation);
            for (var i = At; true; i = i.GetNext())
            {
                if (Result.GetCash(inflation, i) >= expected(i))
                {
                    return i;
                }
            }
        }

        public DateTime FindWhenHasCash(decimal expected)
        {
            if (expected < 0.00M)
            {
                throw new ArgumentOutOfRangeException(nameof(expected), expected, "Should not be negative value");
            }

            var inflation = Inflations.GetInflation(InflationTypes.NoopInflation);

            var at = InitiatedAt;
            at = FindWhenHasAtLeastCash(expected, at);
            at = FindWhenHasAtMostCash(expected, at);

            return at;
        }

        private DateTime FindWhenHasAtLeastCash(decimal expected, DateTime startAt)
        {
            if (expected < 0.00M)
            {
                throw new ArgumentOutOfRangeException(nameof(expected), expected, "Should not be negative value");
            }

            var inflation = Inflations.GetInflation(InflationTypes.NoopInflation);

            decimal actual = decimal.MinValue;
            DateTime at;
            for (at = startAt; actual >= expected; at = at.AddYears(1))
            {
                actual = Result.GetCash(inflation, at);
            }

            return at;
        }

        private DateTime FindWhenHasAtMostCash(decimal expected, DateTime endAt)
        {
            if (expected < 0.00M)
            {
                throw new ArgumentOutOfRangeException(nameof(expected), expected, "Should not be negative value");
            }

            var inflation = Inflations.GetInflation(InflationTypes.NoopInflation);

            decimal actual = decimal.MaxValue;

            // TODO: fixme
            DateTime at = endAt;
            DateTime nextAt = endAt;
            while (Result.GetCash(inflation, nextAt) >= expected)
            {
                at = nextAt;
                nextAt = at.AddMonths(-1);

                actual = Result.GetCash(inflation, nextAt);
            }

            return at;
        }
    }
}
