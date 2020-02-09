using System;
using System.Linq;
using NUnit.Framework;

using Financier.Common.Liabilities;
using Financier.Common.Models;

namespace Financier.Common.Tests.Liabilities
{
    public class FixedRateMortgageTest
    {
        public Home Home { get; }
        public DateTime PurchasedAt => Home.PurchasedAt;
        public Financier.Common.Liabilities.FixedRateMortgage Subject { get; }

        public FixedRateMortgageTest()
        {
            var purchasedAt = new DateTime(2019, 1, 1);
            var downpayment = 50000.00M;
            var mortgageAmount = 200000.00M;
            var mortgageAmountMoney = new Money(mortgageAmount, purchasedAt);
            var preferredInterestRate = 0.0319M;

            Home = new Home("first home", purchasedAt, downpayment);
            Subject = new FixedRateMortgage(Home, mortgageAmountMoney, preferredInterestRate, 300);
        }

        [Test]
        public void Test_PeriodicMonthlyInterestRate()
        {
            Assert.That(
                decimal.Round(Convert.ToDecimal(Subject.PeriodicMonthlyInterestRate), 5),
                Is.EqualTo(0.00264M)
            );
        }

        [Test]
        public void Test_EffectiveAnnualInterestRate()
        {
            Assert.That(
                decimal.Round(Convert.ToDecimal(Subject.EffectiveAnnualInterestRate), 6),
                Is.EqualTo(0.032154M)
            );
        }

        [Test]
        public void Test_PeriodicAnnualInterestRate()
        {
            Assert.That(
                decimal.Round(Convert.ToDecimal(Subject.PeriodicAnnualInterestRate), 6),
                Is.EqualTo(0.03169M)
            );
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
        [TestCase(2, 199122.98)]
        [TestCase(5, 197798.76)]
        [TestCase(10, 195568.30)]
        [TestCase(300, 0)]
        public void Test_GetBalance(int monthCount, decimal expectedBalance)
        {
            Assert.That(
                Subject.GetBalance(PurchasedAt.AddMonths(monthCount))
                , Is.EqualTo(expectedBalance)
            );
        }

        [TestCase(1, 528.17)]
        [TestCase(2, 527.01)]
        [TestCase(5, 523.52)]
        [TestCase(10, 517.65)]
        [TestCase(300, 2.54)]
        public void Test_GetMonthlyInterestPayment(int monthCount, decimal expectedInterestPayment)
        {
            Assert.That(
                Subject.GetMonthlyPayments(PurchasedAt.AddMonths(monthCount))
                    .Select(payment => payment.Interest.Value)
                    .Last()
                , Is.EqualTo(expectedInterestPayment)
            );
        }

        [TestCase(1, 437.93)]
        [TestCase(2, 439.09)]
        [TestCase(5, 442.57)]
        [TestCase(10, 448.45)]
        [TestCase(300, 963.55)]
        public void Test_GetMonthlyPrincipalPayment(int monthCount, decimal expectedPrincipalPayment)
        {
            Assert.That(
                Subject.GetMonthlyPayments(PurchasedAt.AddMonths(monthCount))
                    .Select(payment => payment.Principal.Value)
                    .Last()
                , Is.EqualTo(expectedPrincipalPayment)
            );
        }
    }
}
