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
        [TestCase(InflationTypes.NoopInflation, 2019, 1, 1, 10000.00 + 82000.00 + 89.86 * 0)]
        [TestCase(InflationTypes.NoopInflation, 2019, 1, 2, 10000.00 + 82000.00 + 718.20 + 89.86 * 1)]
        [TestCase(InflationTypes.NoopInflation, 2019, 1, 15, 10000.00 + 82000.00 + 718.20 + 89.86 * 14)]
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
