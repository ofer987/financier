using System;
using NUnit.Framework;

using Financier.Common.Liabilities;
using Financier.Common.Expenses;
using Financier.Common.Models;

namespace Financier.Common.Tests.Expenses.MyHomeTests
{
    // TODO:Rename this file and others to *Tests
    public class WithPrepayableMortgageTest
    {
        public Home Home { get; }
        public ICashFlow CashFlow { get; }
        public FixedRateMortgage BaseMortgage { get; }
        public MyHome Subject { get; }

        public WithPrepayableMortgageTest()
        {
            CashFlow = new DummyCashFlow(89.86M);
            var downpayment = 82000.00M;
            var mortgageAmount = 328000.00M;
            var preferredInterestRate = 0.0319M;
            var purchasedAt = new DateTime(2019, 1, 1);

            Home = new Home("first home", purchasedAt, downpayment);
            BaseMortgage = new FixedRateMortgage(Home, mortgageAmount, preferredInterestRate, 300);
            Subject = MyHome.BuildStatementWithPrepaybleMortgage(BaseMortgage, CashFlow, 2000.00M, 6000.00M);
        }

        [TestCase(2019, 1, 1, -332000)]
        [TestCase(2019, 1, 15, -330023.76)]
        [TestCase(2019, 1, 31, -328586.00)]
        [TestCase(2019, 2, 1, -328496.14)]
        [TestCase(2019, 2, 2, -327686.18)]
        [TestCase(2019, 12, 31, -319255.27 - 4000 + 89.86 * 364)]
        [TestCase(2020, 1, 1, -286456.37 - 4000 + 89.86 * 0)]
        [TestCase(2020, 1, 2, -285628.46 - 4000 + 89.86 * 1)]
        [TestCase(2020, 2, 1, -285628.46 - 4000 + 89.86 * 31)]
        [TestCase(2020, 12, 31, -276375.83 - 4000 + 89.86 * 365)]
        [TestCase(2021, 1, 1, -243575.83 - 4000 + 89.86 * 0 + (89.86 * 366 - 32800))]
        [TestCase(2026, 1, 1, -11410.01)]
        [TestCase(2026, 2, 1, -7059.99)]
        [TestCase(2026, 3, 1, -2975.42)]
        [TestCase(2026, 4, 1, 1382.88)]
        [TestCase(2026, 5, 1, 5655.46)]
        [TestCase(2026, 6, 1, 9746.38)]
        [TestCase(2026, 7, 1, 12442.18)]
        [TestCase(2026, 8, 1, 15227.84)]
        [TestCase(2026, 9, 1, 18013.50)]
        [TestCase(2026, 10, 1, 20709.30)]
        public void Test_GetBalance(int year, int month, int day, decimal expected)
        {
            Assert.That(Subject.GetBalance(new DateTime(year, month, day)), Is.EqualTo(expected));
        }
    }
}
