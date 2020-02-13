using System;
using NUnit.Framework;

using Financier.Common.Liabilities;
using Financier.Common.Expenses;
using Financier.Common.Models;

namespace Financier.Common.Tests.Expenses
{
    // TODO:Rename this file and others to *Tests
    public class BalanceSheetTest
    {
        public Home Home { get; }
        public ICashFlow CashFlow { get; }
        public IMortgage Mortgage { get; }
        public BalanceSheet Subject { get; }

        public BalanceSheetTest()
        {
            var purchasedAt = new DateTime(2019, 1, 1);
            var downpayment = 82000.00M;
            var mortgageAmount = 328000.00M;
            var mortgageAmountMoney = new Money(mortgageAmount, purchasedAt);
            var preferredInterestRate = 0.0319M;

            var initiatedAt = purchasedAt;
            var initialCash = new Money(10000.00M, initiatedAt);
            var initialDebt = new Money(5000.00M, initiatedAt);
            CashFlow = new DummyCashFlow(89.86M);
            Mortgage = new FixedRateMortgage(mortgageAmountMoney, preferredInterestRate, 300, purchasedAt);

            Home = new Home("first home", purchasedAt, downpayment, Mortgage);
            Subject = new BalanceSheet(initialCash, initialCash, CashFlow, initiatedAt, Home);
        }

        // TODO: calculate just the mortgage payments
        [TestCase(InflationTypes.NoopInflation, 2019, 1, 1, 1584.84)]
        [TestCase(InflationTypes.CompoundYearlyInflation, 2019, 1, 1, 1584.84)]
        [TestCase(2019, 1, 15, -330023.76)]
        [TestCase(2019, 1, 31, -328586.00)]
        [TestCase(2019, 2, 1, -328496.14)]
        [TestCase(2019, 2, 2, -327686.18)]
        [TestCase(2019, 12, 31, 32800.00)]
        [TestCase(2020, 1, 1, 1584.84)]
        [TestCase(2020, 1, 2, -285628.46 - 4000 + 89.86 * 1)]
        [TestCase(2020, 2, 1, -285628.46 - 4000 + 89.86 * 31)]
        [TestCase(2020, 12, 31, 32800.00)]
        [TestCase(2021, 1, 1, -243575.83 - 4000 + 89.86 * 0 + (89.86 * 366 - 32800))]
        [TestCase(2026, 1, 1, -11410.01)]
        [TestCase(2026, 2, 1, -7059.99)]
        [TestCase(2026, 3, 1, -2975.42)]
        [TestCase(2026, 4, 1, 1382.87)]
        [TestCase(2026, 5, 1, 5655.47)]
        [TestCase(2026, 6, 1, 9746.38)]
        [TestCase(2026, 7, 1, 12442.18)]
        [TestCase(2026, 8, 1, 15227.84)]
        [TestCase(2026, 9, 1, 18013.50)]
        [TestCase(2026, 10, 1, 1584.84)]
        public void Test_GetAssets(InflationTypes inflationType, int year, int month, int day, decimal expected)
        {
            var inflation = Inflations.GetInflation(inflationType);

            Assert.That(
                Subject.GetAssets(inflation, new DateTime(year, month, day)),
                Is.EqualTo(expected)
            );
        }

        public void Test_GetLiabilities(InflationTypes inflationType, int year, int month, int day, decimal expected)
        {
            var inflation = Inflations.GetInflation(inflationType);
            Assert.That(
                Subject.GetLiabilities(inflation, new DateTime(year, month, day)),
                Is.EqualTo(expected)
            );
        }

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
