using System;

using Financier.Common.Models;
using Financier.Common.Tests.Expenses;
using NUnit.Framework;

namespace Financier.Common.Tests.Calculations
{
    public class FixedRateMortgage : Fixture
    {
        public Home Home { get; }
        public Financier.Common.Calculations.FixedRateMortgage Subject { get; }

        public FixedRateMortgage()
        {
            var downpayment = 50000.00M;
            var mortgageAmount = 200000.00M;
            var preferredInterestRate = 0.0319M;
            Home = new Home("first home", new DateTime(2019, 1, 1), downpayment);
            Subject = new Financier.Common.Calculations.FixedRateMortgage(Home, mortgageAmount, preferredInterestRate, 300);
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
        [TestCase(2, 199122.99)]
        [TestCase(5, 197798.76)]
        [TestCase(10, 195568.30)]
        [TestCase(300, 0)]
        public void Test_GetBalance(int monthCount, decimal expectedBalance)
        {
            Assert.That(Subject.GetBalance(monthCount), Is.EqualTo(expectedBalance));
        }

        [TestCase(1, 528.17)]
        [TestCase(2, 527.01)]
        [TestCase(5, 523.52)]
        [TestCase(10, 517.65)]
        [TestCase(300, 2.54)]
        public void Test_GetMonthlyInterestPayment(int monthCount, decimal expectedInterestPayment)
        {
            Assert.That(Subject.GetMonthlyInterestPayment(monthCount), Is.EqualTo(expectedInterestPayment));
        }
    }
}
