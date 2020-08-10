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

            var initialCash = 10000.00M;
            var initialDebt = 5000.00M;

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
            }

            // Sell the first home
            {
                var soldAt = new DateTime(2020, 1, 3);
                Subject.Sell(FirstHome, 500000.00M, soldAt);
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
            }
        }

        [TestCase(InflationTypes.NoopInflation, 2019, 1, 1, 2019, 1, 2, - 410000.00 + 89.86 * 1 - (1000.00 + 8500 + 800))]
        [TestCase(InflationTypes.NoopInflation, 2019, 1, 1, 2019, 1, 15, - 410000.00 + 89.86 * 14 - (1000.00 + 8500 + 800))]
        [TestCase(InflationTypes.NoopInflation, 2019, 1, 2, 2019, 1, 15, + 89.86 * 13)]
        [TestCase(InflationTypes.NoopInflation, 2019, 1, 1, 2020, 2, 4, - 410000.00 + 500000.00 - 410000.00 + 89.86 * 399 - (1000.00 + 8500 + 800) - (25000 + 1000 + 1000) - 317588.78 - (1000.00 + 8500 + 800))]
        [TestCase(InflationTypes.NoopInflation, 2020, 1, 4, 2020, 2, 4, - 410000.00 + 89.86 * 31 - (1000.00 + 8500 + 800))]
        public void Test_BalanceSheet_GetCash(InflationTypes inflationType, int startYear, int startMonth, int startDay, int endYear, int endMonth, int endDay, decimal expected)
        {
            var inflation = Inflations.GetInflation(inflationType);

            Assert.That(
                Subject.GetCash(
                    inflation,
                    new DateTime(startYear, startMonth, startDay),
                    new DateTime(endYear, endMonth, endDay)
                ),
                Is.EqualTo(expected)
            );
        }

        [TestCase(InflationTypes.NoopInflation, 2019, 1, 2, - 410000.00 + 10000.00 - 5000.00 + 89.86 * 1 - (1000.00 + 8500 + 800))]
        [TestCase(InflationTypes.NoopInflation, 2019, 1, 15, - 410000.00 + 10000.00 - 5000.00 + 89.86 * 14 - (1000.00 + 8500 + 800))]
        [TestCase(InflationTypes.NoopInflation, 2019, 2, 3, - 410000.00 + 10000.00 - 5000.00 + 89.86 * 33 - (1000.00 + 8500 + 800))]
        [TestCase(InflationTypes.NoopInflation, 2020, 1, 2, -410000.00 + 10000.00 - 5000.00 + 89.86 * 366 - (1000.00 + 8500 + 800))]
        [TestCase(InflationTypes.NoopInflation, 2020, 1, 3, - 410000.00 + 10000.00 - 5000.00 + 89.86 * 367 - (1000.00 + 8500 + 800))]
        [TestCase(InflationTypes.NoopInflation, 2020, 1, 4, - 410000.00 + 500000.00 + 10000.00 - 5000.00 + 89.86 * 368 - (1000.00 + 8500 + 800) - (25000 + 1000 + 1000) - 317588.78)]
        [TestCase(InflationTypes.NoopInflation, 2020, 2, 3, - 410000.00 + 500000.00 + 10000.00 - 5000.00 + 89.86 * 398 - (1000.00 + 8500 + 800) - (25000 + 1000 + 1000) - 317588.78)]
        [TestCase(InflationTypes.NoopInflation, 2020, 2, 4, - 410000.00 + 500000.00 - 410000.00 + 10000.00 - 5000.00 + 89.86 * 399 - (1000.00 + 8500 + 800) - (25000 + 1000 + 1000) - 317588.78 - (1000.00 + 8500 + 800))]
        public void Test_BalanceSheet_GetCashAt(InflationTypes inflationType, int year, int month, int day, decimal expected)
        {
            var inflation = Inflations.GetInflation(inflationType);

            Assert.That(
                Subject.GetCashAt(inflation, new DateTime(year, month, day)),
                Is.EqualTo(expected)
            );
        }

        [TestCase(2019, 1, 2, 0.00)]
        [TestCase(2019, 1, 15, 0.00)]
        [TestCase(2019, 2, 3, 0.00)]
        [TestCase(2020, 1, 2, 0.00)]
        [TestCase(2020, 1, 3, 0.00)]
        [TestCase(2020, 1, 4, 0.00)]
        [TestCase(2020, 2, 3, 0.00)]
        [TestCase(2020, 2, 4, 0.00)]
        public void Test_BalanceSheet_GetValueAt(int year, int month, int day, decimal expected)
        {
            Assert.That(
                Subject.GetValueAt(new DateTime(year, month, day)),
                Is.EqualTo(expected)
            );
        }

        [TestCase(2019, 1, 2, 1584.39)]
        [TestCase(2019, 1, 15, 1584.39)]
        [TestCase(2019, 2, 3, 1584.39 * 2)]
        [TestCase(2020, 1, 2, 1584.39 * 13)]
        [TestCase(2020, 1, 3, 1584.39 * 13)]
        [TestCase(2020, 1, 4, 1584.39 * 13)]
        [TestCase(2020, 2, 3, 1584.39 * 13)]
        [TestCase(2020, 2, 4, 1584.39 * 13 + 1584.39)]
        public void Test_BalanceSheet_GetCostAt(int year, int month, int day, decimal expected)
        {
            Assert.That(
                Subject.GetCostAt(new DateTime(year, month, day)),
                Is.InRange(
                    expected * Convert.ToDecimal(0.9999),
                    expected * Convert.ToDecimal(1.0001)
                )
            );
        }

        [TestCase(InflationTypes.NoopInflation, 2019, 1, 2, - 410000.00 + 10000.00 - 5000.00 + 89.86 * 1 - (1000.00 + 8500 + 800) + 0.00 - 1584.39)]
        [TestCase(InflationTypes.NoopInflation, 2020, 2, 4, - 410000.00 + 500000.00 - 410000.00 + 10000.00 - 5000.00 + 89.86 * 399 - (1000.00 + 8500 + 800) - (25000 + 1000 + 1000) - 317588.78 - (1000.00 + 8500 + 800) + 0.00 - (1584.39 * 13 + 1584.39))]
        public void Test_BalanceSheet_GetNetWorthAt(InflationTypes inflationType, int year, int month, int day, decimal expected)
        {
            var inflation = Inflations.GetInflation(inflationType);

            Assert.That(
                Subject.GetNetWorthAt(inflation, new DateTime(year, month, day)),
                // Is.EqualTo(expected)
                Is.InRange(
                    Convert.ToDecimal(1.0001) * expected,
                    Convert.ToDecimal(0.9999) * expected
                )
            );
        }

    }
}
