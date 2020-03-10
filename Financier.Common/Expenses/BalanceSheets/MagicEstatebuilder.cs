using System;
using System.Linq;
using System.Collections.Generic;

using Financier.Common.Models;
using Financier.Common.Liabilities;
using Financier.Common.Expenses.Actions;
using Financier.Common.Extensions;

namespace Financier.Common.Expenses.BalanceSheets
{
    public class MagicEstateBuilder
    {
        public DateTime InitiatedAt { get; private set; }
        private DateTime At { get; set; }
        private Activity Result { get; set; }

        public MagicEstateBuilder(ICashFlow cashFlow, DateTime initiatedAt)
        {
            Result = new Activity(cashFlow, initiatedAt);
            InitiatedAt = At = initiatedAt;
        }

        public IEnumerable<Home> GetHomes()
        {
            return Result.GetOwnedProducts()
                .Where(product => product.GetType() == typeof(Home))
                .Cast<Home>();
        }

        public MagicEstateBuilder SetInitialCash(decimal cash)
        {
            Result.InitialCash = new Money(cash, InitiatedAt);

            return this;
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

        public MagicEstateBuilder AddCondoWithFixedRateMortgageAtDownPaymentPercentage(decimal purchasePriceAtInitiation, decimal downPaymentRate, decimal interestRate = 0.0319M)
        {
            return AddHomeWithFixedRateMortgageAtDownPaymentPercentage(0.05M, purchasePriceAtInitiation, downPaymentRate, interestRate);
        }

        private MagicEstateBuilder AddHomeWithFixedRateMortgageAtDownPaymentPercentage(decimal appreciationRate, decimal purchasePriceAtInitiation, decimal downPaymentRate, decimal interestRate = 0.0319M)
        {
            if (purchasePriceAtInitiation < 0.00M)
            {
                throw new ArgumentOutOfRangeException(nameof(purchasePriceAtInitiation), purchasePriceAtInitiation, "Should be at or greater than 0.00");
            }

            if (downPaymentRate < 0.00M || downPaymentRate > 1.00M)
            {
                throw new ArgumentOutOfRangeException(nameof(downPaymentRate), downPaymentRate, "Should be between 0.00 and 1.00 (inclusive)");
            }

            // Baseline for prices
            var initiatedAt = At;

            // Does not take into account inflation
            Func<DateTime, decimal> downPaymentAmountFunc = (at) =>
            {
                return new Money(
                    downPaymentRate * purchasePriceAtInitiation,
                    initiatedAt
                ).GetValueAt(Inflations.GetInflation(InflationTypes.CompoundYearlyInflation, 0.05M), at).Value;
            };

            var purchasedAt = HasCashAt(downPaymentAmountFunc, initiatedAt);
            // Console.WriteLine($"Purchased at {purchasedAt} with {Result.GetCash(Inflations.NoopInflation, purchasedAt)}");
            var downPaymentAmount = new Money(
                downPaymentRate * purchasePriceAtInitiation,
                initiatedAt
            ).GetValueAt(Inflations.GetInflation(InflationTypes.CompoundYearlyInflation, 0.05M), purchasedAt);
            var mortgageAmount = new Money(
                (1 - downPaymentRate) * purchasePriceAtInitiation,
                initiatedAt
            ).GetValueAt(Inflations.GetInflation(InflationTypes.CompoundYearlyInflation, 0.05M), purchasedAt);
            var fullAmount = new Money(
                purchasePriceAtInitiation,
                initiatedAt
            ).GetValueAt(Inflations.GetInflation(InflationTypes.CompoundYearlyInflation, 0.05M), purchasedAt);
            var mortgage = Mortgages.GetFixedRateMortgage(
                mortgageAmount,
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
            var mortgage = home.Financing;
            Result.Sell(mortgage, mortgage.GetBalance(soldAt), soldAt);

            Result.Sell(home, soldPrice, soldAt);

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
                try
                {
                    if (Result.GetCash(inflation, i) >= expected(i))
                    {
                        return i;
                    }
                }
                catch (ArgumentOutOfRangeException)
                {
                }
                catch (OverflowException)
                {
                    // Console.WriteLine($"Crashed at {i}");

                    throw;
                }
            }
        }
    }
}
