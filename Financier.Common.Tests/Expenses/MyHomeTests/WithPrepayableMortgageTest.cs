using System;
using NUnit.Framework;

using Financier.Common.Liabilities;
using Financier.Common.Expenses;
using Financier.Common.Models;

namespace Financier.Common.Tests.Liabilities
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
            CashFlow = new DummyCashFlow(18000.00M);
            var downpayment = 82000.00M;
            var mortgageAmount = 328000.00M;
            var preferredInterestRate = 0.0319M;
            var purchasedAt = new DateTime(2019, 1, 1);
            Home = new Home("first home", purchasedAt, downpayment);
            BaseMortgage = new FixedRateMortgage(Home, mortgageAmount, preferredInterestRate, 300);
            Subject = MyHome.BuildStatementWithPrepaybleMortgage(BaseMortgage, CashFlow, 2000.00M, 6000.00M);
        }

        [TestCase(2019, 1, 1, 332000)]
        [TestCase()]
        public void Test_GetBalance(int year, int month, int day, decimal expected)
        {
            Assert.That(Subject.GetBalance(new DateTime(year, month, day)), Is.EqualTo(expected));
        }

        [TestCase(1, 199562.07)]
        [TestCase(2, 199122.99)]
        [TestCase(5, 197798.76)]
        [TestCase(10, 195568.30)]
        [TestCase(13, 164215.84)]
        [TestCase(14, 163683.41)]
        [TestCase(18, 126447.16)]
        public void Test_GetBalance(int monthCount, decimal expectedBalance)
        {
            Assert.That(Subject.GetBalance(monthCount), Is.EqualTo(expectedBalance));
        }

        [TestCase(1, 528.17)]
        [TestCase(2, 527.01)]
        [TestCase(5, 523.52)]
        [TestCase(10, 517.65)]
        [TestCase(13, 514.09)]
        [TestCase(18, 335.59)]
        public void Test_GetMonthlyInterestPayment(int monthCount, decimal expectedInterestPayment)
        {
            Assert.That(Subject.GetMonthlyInterestPayment(monthCount), Is.EqualTo(expectedInterestPayment));
        }

        [TestCase(1, 437.93)]
        [TestCase(2, 439.09)]
        [TestCase(5, 442.57)]
        [TestCase(10, 448.45)]
        [TestCase(13, 30452.01)]
        [TestCase(17, 35536.66)]
        [TestCase(18, 630.50)]
        public void Test_GetMonthlyPrincipalPayment(int monthCount, decimal expectedPrincipalPayment)
        {
            Assert.That(Subject.GetMonthlyPrincipalPayment(monthCount), Is.EqualTo(expectedPrincipalPayment));
        }
    }
}
