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

        public MagicEstateBuilder AddCondoWithFixedRateMortgageAtDownPaymentPercentage(string name, decimal purchasePriceAtInitiation, decimal downPaymentRate, decimal interestRate = 0.0319M)
        {
            return AddHomeWithFixedRateMortgageAtDownPaymentPercentage(name, 0.05M, purchasePriceAtInitiation, downPaymentRate, interestRate);
        }

        private MagicEstateBuilder AddHomeWithFixedRateMortgageAtDownPaymentPercentage(string name, decimal appreciationRate, decimal purchasePriceAtInitiation, decimal downPaymentRate, decimal interestRate = 0.0319M)
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
            var homeInflation = Inflations.CondoPriceIndex;

            var purchasedAt = new CashFinder(
                Result,
                initiatedAt
            ).HasAvailableCashAt(
                downPaymentRate * purchasePriceAtInitiation,
                homeInflation
            );
            var downPaymentAmount = new Money(
                downPaymentRate * purchasePriceAtInitiation,
                initiatedAt
            ).GetValueAt(homeInflation, purchasedAt);
            var mortgageAmount = new Money(
                (1 - downPaymentRate) * purchasePriceAtInitiation,
                initiatedAt
            ).GetValueAt(homeInflation, purchasedAt);
            var fullAmount = new Money(
                purchasePriceAtInitiation,
                initiatedAt
            ).GetValueAt(homeInflation, purchasedAt);
            var mortgage = Mortgages.GetFixedRateMortgage(
                mortgageAmount,
                0.0319M,
                300,
                purchasedAt,
                downPaymentAmount
            );
            var home = new Home(
                name,
                purchasedAt,
                fullAmount,
                downPaymentAmount,
                mortgage
            );

            Result.Buy(home, purchasedAt);

            At = purchasedAt.GetNext();

            return this;
        }

        public MagicEstateBuilder SellHome(Home home, DateTime soldAt, Money soldPrice)
        {
            Result.Sell(home, soldPrice, soldAt);

            return this;
        }

        public MagicEstateBuilder SellHome(string name, DateTime soldAt, Money soldPrice)
        {
            Home home;
            try
            {
                home = GetHomes().First(item => item.Name == name);
            }
            catch (InvalidOperationException)
            {
                throw new ArgumentException("Either home has already been sold or home has never been purchased", nameof(name));
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ArgumentException("Either home has already been sold or home has never been purchased", nameof(name));
            }

            if (soldAt <= home.PurchasedAt)
            {
                throw new ArgumentException("A home should be sold after it has been purchased", nameof(name));
            }

            return SellHome(home, soldAt, soldPrice);
        }

        public Activity Build()
        {
            return Result;
        }
    }
}
