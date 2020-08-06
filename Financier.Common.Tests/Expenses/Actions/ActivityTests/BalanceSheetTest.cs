using System;
using NUnit.Framework;

using Financier.Common.Liabilities;
using Financier.Common.Expenses;
using Financier.Common.Expenses.Actions;
using Financier.Common.Models;

namespace Financier.Common.Tests.Expenses.Actions.ActivityTests
{
    // TODO:Rename this file and others to *Tests
    // TODO: move tests to ActivityTests/BalanceSheets
    public class BalanceSheetTest
    {
        public DateTime InitiatedAt => Subject.InitiatedAt;
        public ICashFlow CashFlow { get; private set; }
        public Activity Subject { get; private set; }
        public Home FirstHome { get; private set; }
        public Home SecondHome { get; private set; }

        [SetUp]
        public void Init()
        {
            var initiatedAt = new DateTime(2019, 1, 1);
            var downpayment = 82000.00M;
            var mortgageAmount = 328000.00M;
            var preferredInterestRate = 0.0319M;

            var initialCash = new Money(10000.00M, initiatedAt);
            var initialDebt = new Money(5000.00M, initiatedAt);

            CashFlow = new DummyCashFlow(89.86M);
            Subject = new Activity(initialCash, initialDebt, CashFlow, initiatedAt);

            {
                var purchasedAt = initiatedAt;
                var mortgage = new FixedRateMortgage(
                    mortgageAmount,
                    preferredInterestRate,
                    300,
                    purchasedAt
                );
                FirstHome = new Home(
                    "first home",
                    purchasedAt,
                    downpayment + mortgageAmount,
                    downpayment,
                    mortgage
                );

                Subject.Buy(FirstHome, purchasedAt);
                Subject.Buy(FirstHome.Financing, purchasedAt);
            }

            // Sell the first home
            {
                var soldAt = new DateTime(2020, 1, 3);
                Subject.Sell(FirstHome, new Money(500000.00M, soldAt), soldAt);
                Subject.Sell(
                    FirstHome.Financing,
                    0.00M - FirstHome.Financing.GetBalance(soldAt),
                    soldAt
                );
            }

            {
                var purchasedAt = new DateTime(2020, 2, 3);
                var mortgage = new FixedRateMortgage(
                    mortgageAmount,
                    preferredInterestRate,
                    300,
                    purchasedAt
                );
                SecondHome = new Home(
                    "second home",
                    purchasedAt,
                    downpayment + mortgageAmount,
                    downpayment,
                    mortgage
                );
                Subject.Buy(SecondHome, purchasedAt);
                Subject.Buy(mortgage, purchasedAt);
            }
        }

        [TestCase(InflationTypes.NoopInflation, 2019, 1, 2, 10000.00 + 712.46 + 89.86 * 1 - (1000.00 + 8500 + 800))]
        [TestCase(InflationTypes.NoopInflation, 2019, 1, 15, 10000.00 + 712.46 + 89.86 * 14 - (1000.00 + 8500 + 800))]
        [TestCase(InflationTypes.NoopInflation, 2019, 2, 3, 10000.00 + 712.46 + 714.36 + 89.86 * 33 - (1000.00 + 8500 + 800))]
        [TestCase(InflationTypes.NoopInflation, 2020, 1, 2, 10000.00 + 9411.22 + 89.86 * 366 - (1000.00 + 8500 + 800))]
        [TestCase(InflationTypes.NoopInflation, 2020, 1, 3, 10000.00 + 9411.22 + 89.86 * 367 - (1000.00 + 8500 + 800))]
        [TestCase(InflationTypes.NoopInflation, 2020, 1, 4, 10000.00 - 82000 + 89.86 * 368 + 0.95 * 500000.00 - 318588.78 - (1000.00 + 8500 + 800))]
        [TestCase(InflationTypes.NoopInflation, 2020, 2, 3, 10000.00 - 82000 + 89.86 * 398 + 0.95 * 500000.00 - 318588.78 - (1000.00 + 8500 + 800))]
        [TestCase(InflationTypes.NoopInflation, 2020, 2, 4, 10000.00 - 82000 + 89.86 * 399 + 0.95 * 500000.00 - 318588.78 + 712.46 - 2 * (1000.00 + 8500 + 800))]
        public void Test_GetAssets(InflationTypes inflationType, int year, int month, int day, decimal expected)
        {
            var inflation = Inflations.GetInflation(inflationType);

            Assert.That(
                Subject.GetAssets(inflation, new DateTime(year, month, day)),
                Is.EqualTo(expected)
            );
        }

        [TestCase(InflationTypes.NoopInflation, 2019, 1, 2, 5000.00 + 327287.54)]
        [TestCase(InflationTypes.NoopInflation, 2019, 1, 15, 5000.00 + 327287.54)]
        [TestCase(InflationTypes.NoopInflation, 2019, 2, 3, 5000.00 + 326573.18)]
        [TestCase(InflationTypes.NoopInflation, 2020, 1, 2, 5000.00 + 318588.78)]
        [TestCase(InflationTypes.NoopInflation, 2020, 1, 3, 5000.00 + 318588.78)]
        [TestCase(InflationTypes.NoopInflation, 2020, 1, 4, 5000.00)]
        [TestCase(InflationTypes.NoopInflation, 2020, 2, 3, 5000.00)]
        [TestCase(InflationTypes.NoopInflation, 2020, 2, 4, 5000.00 + 327287.54)]
        public void Test_GetLiabilities(InflationTypes inflationType, int year, int month, int day, decimal expected)
        {
            var inflation = Inflations.GetInflation(inflationType);
            Assert.That(
                Subject.GetLiabilities(inflation, new DateTime(year, month, day)),
                Is.EqualTo(expected)
            );
        }

        [TestCase(InflationTypes.NoopInflation, 2019, 1, 2, 10000.00 + 712.46 + 89.86 * 1 - (5000.00 + 327287.54) - (1000.00 + 8500 + 800))]
        [TestCase(InflationTypes.NoopInflation, 2020, 2, 4, (10000.00 - 82000.00 + 89.86 * 399 + 0.95 * 500000.00 - 318588.78 + 712.46) - (5000.00 + 327287.54) - 2 * (1000.00 + 8500 + 800))]
        public void Test_GetNetWorth(InflationTypes inflationType, int year, int month, int day, decimal expected)
        {
            var inflation = Inflations.GetInflation(inflationType);

            Assert.That(
                Subject.GetNetWorth(inflation, new DateTime(year, month, day)),
                Is.EqualTo(expected)
            );
        }

    }
}
