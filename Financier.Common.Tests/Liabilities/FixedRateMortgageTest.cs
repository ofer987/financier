using System;
using System.Linq;
using NUnit.Framework;

using Financier.Common.Liabilities;
using Financier.Common.Models;

namespace Financier.Common.Tests.Liabilities
{
    public class FixedRateMortgageTest
    {
        public DateTime PurchasedAt => Subject.InitiatedAt;
        public Financier.Common.Liabilities.FixedRateMortgage Subject { get; }

        public FixedRateMortgageTest()
        {
            var purchasedAt = new DateTime(2019, 1, 1);
            var mortgageAmount = 328000.00M;
            var mortgageAmountMoney = new Money(mortgageAmount, purchasedAt);
            var preferredInterestRate = 0.0319M;

            Subject = new FixedRateMortgage(mortgageAmountMoney, preferredInterestRate, 300, purchasedAt);
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
                decimal.Round(Convert.ToDecimal(Subject.PeriodicAnnualInterestRate), 4),
                Is.EqualTo(0.0319M)
            );
        }

        [Test]
        public void Test_MonthlyPayment()
        {
            Assert.That(
                decimal.Round(
                    Convert.ToDecimal(Subject.MonthlyPayment),
                    2
                ),
                Is.EqualTo(1584.40M)
            );
        }

        [TestCase(2019, 1, 1, 328000)]
        [TestCase(2019, 1, 15, 327287.54)]
        [TestCase(2019, 1, 31, 327287.54)]
        [TestCase(2019, 2, 1, 327287.54)]
        [TestCase(2019, 2, 2, 326573.18)]
        [TestCase(2019, 12, 31, 319324.30)]
        [TestCase(2020, 1, 1, 319324.30)]
        [TestCase(2020, 1, 2, 318588.78)]
        [TestCase(2044, 1, 1, 1659.12)]
        [TestCase(2044, 2, 1, 79.13)]
        [TestCase(2044, 3, 1, 0)]
        [TestCase(2045, 1, 1, 0)]
        public void Test_GetBalance(int year, int month, int day, decimal expected)
        {
            Assert.That(
                Subject.GetBalance(new DateTime(year, month, day)),
                Is.EqualTo(expected)
            );
        }

        [TestCase(1, 871.93)]
        [TestCase(2, 870.04)]
        [TestCase(5, 864.33)]
        [TestCase(10, 854.71)]
        [TestCase(300, 8.60)]
        public void Test_GetMonthlyInterestPayment(int monthCount, decimal expectedInterestPayment)
        {
            Assert.That(
                Subject.GetMonthlyPayments()
                    .Select(payment => payment.Interest.Value)
                    .ToList()[monthCount]
                , Is.EqualTo(expectedInterestPayment)
            );
        }

        [TestCase(1, 712.46)]
        [TestCase(2, 714.36)]
        [TestCase(5, 720.07)]
        [TestCase(10, 729.69)]
        [TestCase(300, 1575.80)]
        public void Test_GetMonthlyPrincipalPayment(int monthCount, decimal expectedPrincipalPayment)
        {
            Assert.That(
                Subject.GetMonthlyPayments()
                    .Select(payment => payment.Principal.Value)
                    .ToList()[monthCount]
                , Is.EqualTo(expectedPrincipalPayment)
            );
        }

        [TestCase(0.0319, 300, 0.002640836774)]
        public void Test_PeriodicMonthlyInterestRate(decimal interestRate, int amortisationPeriodInMonths, decimal expected)
        {
            var purchasedAt = new DateTime(2019, 1, 1);
            var subject = new FixedRateMortgage(new Money(100000, purchasedAt), interestRate, 300, purchasedAt);

            Assert.That(
                decimal.Round(Convert.ToDecimal(subject.PeriodicMonthlyInterestRate), 12),
                Is.EqualTo(expected)
            );
        }

        [TestCase(2020, 1, 31, 2020, 1, 28)]
        [TestCase(2020, 1, 28, 2020, 1, 28)]
        [TestCase(2020, 2, 28, 2020, 2, 28)]
        [TestCase(2019, 1, 29, 2019, 1, 28)]
        [TestCase(2020, 12, 31, 2020, 12, 28)]
        [TestCase(2010, 12, 31, 2010, 12, 28)]
        public void Test_Constructor(int year, int month, int day, int expectedYear, int expectedMonth, int expectedDay)
        {
            var date = new DateTime(year, month, day);
            var money = new Money(0.00M, date);

            var mortgage = new FixedRateMortgage(money, 0.0314M, 300, date);

            Assert.That(mortgage.InitiatedAt.Year, Is.EqualTo(expectedYear));
            Assert.That(mortgage.InitiatedAt.Month, Is.EqualTo(expectedMonth));
            Assert.That(mortgage.InitiatedAt.Day, Is.EqualTo(expectedDay));
        }
    }
}
