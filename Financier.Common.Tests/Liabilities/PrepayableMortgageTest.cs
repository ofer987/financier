using System;
using System.Linq;
using NUnit.Framework;

using Financier.Common.Liabilities;
using Financier.Common.Models;

namespace Financier.Common.Tests.Liabilities
{
    public class PrepayableMortgageTest
    {
        public Home Home { get; }
        public DateTime PurchasedAt => Home.PurchasedAt;
        public Financier.Common.Liabilities.FixedRateMortgage Mortgage { get; }
        public Financier.Common.Liabilities.PrepayableMortgage Subject { get; }

        public PrepayableMortgageTest()
        {
            var downpayment = 50000.00M;
            var mortgageAmount = 200000.00M;
            var preferredInterestRate = 0.0319M;
            var purchasedAt = new DateTime(2019, 1, 1);
            Home = new Home("first home", purchasedAt, downpayment);
            Mortgage = new FixedRateMortgage(Home, mortgageAmount, preferredInterestRate, 300);
            Subject = new Financier.Common.Liabilities.PrepayableMortgage(Mortgage, 0.20M);
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
            Assert.That(Subject.GetBalance(PurchasedAt.AddMonths(monthCount)), Is.EqualTo(expectedBalance));
        }

        [TestCase(1, 528.17)]
        [TestCase(2, 527.01)]
        [TestCase(5, 523.52)]
        [TestCase(10, 517.65)]
        [TestCase(13, 514.09)]
        [TestCase(18, 335.59)]
        public void Test_GetMonthlyInterestPayment(int monthCount, decimal expectedInterestPayment)
        {
            Assert.That(
                Subject.GetMonthlyPayments(PurchasedAt.AddMonths(monthCount))
                    .Select(payment => payment.Interest)
                    .Last()
                , Is.EqualTo(expectedInterestPayment)
            );
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
            Assert.That(
                Subject.GetMonthlyPayments(PurchasedAt.AddMonths(monthCount))
                    .Select(payment => payment.Principal)
                    .Last()
                , Is.EqualTo(expectedPrincipalPayment)
            );
        }
    }
}
