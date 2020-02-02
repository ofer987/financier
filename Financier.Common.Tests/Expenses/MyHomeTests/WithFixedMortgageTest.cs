using System;
using NUnit.Framework;

using Financier.Common.Liabilities;
using Financier.Common.Expenses;
using Financier.Common.Models;

namespace Financier.Common.Tests.Expenses.MyHomeTests
{
    // TODO: Rename this file and others to *Tests
    public class WithFixedMortgageTest
    {
        public Home Home { get; }
        public ICashFlow CashFlow { get; }
        public FixedRateMortgage Mortgage { get; }
        public MyHome Subject { get; }

        public WithFixedMortgageTest()
        {
            CashFlow = new DummyCashFlow(89.86M);
            var purchasedAt = new DateTime(2019, 1, 1);
            var downpayment = 82000.00M;
            var mortgageAmount = 328000.00M;
            var mortgageAmountMoney = new Money(mortgageAmount, purchasedAt);
            var preferredInterestRate = 0.0319M;

            Home = new Home("first home", purchasedAt, downpayment);
            Mortgage = new FixedRateMortgage(Home, mortgageAmountMoney, preferredInterestRate, 300);
            Subject = MyHome.BuildStatementWithMortgage(Mortgage, CashFlow, 2000.00M, 6000.00M);
        }

        [TestCase(2019, 1, 1, -332000)]
        [TestCase(2019, 1, 15, -330023.76)]
        [TestCase(2019, 1, 31, -328586.00)]
        [TestCase(2019, 2, 1, -328496.14)]
        [TestCase(2019, 2, 2, -327686.18)]
        [TestCase(2019, 12, 31, -319255.27 - 4000 + 89.86 * 364)]
        [TestCase(2020, 1, 1, -319255.27 - 4000 + 89.86 * 365)]
        [TestCase(2020, 1, 2, -318513.97 - 4000 + 89.86 * 366)]
        [TestCase(2020, 2, 1, -318513.97 - 4000 + 89.86 * 396)]
        [TestCase(2044, 1, 1, 0 - 4000 + 89.86 * 9131)]
        public void Test_GetBalance(int year, int month, int day, decimal expected)
        {
            Assert.That(Subject.GetBalance(new DateTime(year, month, day)), Is.EqualTo(expected));
        }
    }
}
