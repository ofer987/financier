using System;
using NUnit.Framework;

using Financier.Common.Models;

namespace Financier.Common.Tests.Liabilities
{
    public class PrepayableMortgage
    {
        public Home Home { get; }
        public Financier.Common.Liabilities.FixedRateMortgage Mortgage { get; }
        public Financier.Common.Liabilities.PrepayableMortgage Subject { get; }

        public PrepayableMortgage()
        {
            var downpayment = 50000.00M;
            var mortgageAmount = 200000.00M;
            var preferredInterestRate = 0.0319M;
            Home = new Home("first home", new DateTime(2019, 1, 1), downpayment);
            Mortgage = new Financier.Common.Liabilities.FixedRateMortgage(Home, mortgageAmount, preferredInterestRate, 300);
            Subject = new Financier.Common.Liabilities.PrepayableMortgage(Mortgage, new DateTime(2019, 1, 1), 0.20M);
            Subject.AddPrepayment(new DateTime(2020, 1, 1), 30000.00M);
            Subject.AddPrepayment(new DateTime(2020, 5, 1), 35000.00M);
        }

        [Test]
        public void Test_MonthlyPayment()
        {
            Assert.That(
                decimal.Round(Convert.ToDecimal(Subject.MonthlyPayment), 6),
                Is.EqualTo(966.096364M)
            );
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
