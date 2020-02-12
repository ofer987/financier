using System;
using System.Linq;
using NUnit.Framework;

using Financier.Common.Liabilities;
using Financier.Common.Expenses;
using Financier.Common.Models;

namespace Financier.Common.Tests.Expenses.PrepayableMortgageBuilderTests
{
    // TODO:Rename this file and others to *Tests
    public class BuildTest
    {
        public PrepayableMortgageBuilder Subject { get; }
        public Home Home { get; }
        public FixedRateMortgage BaseMortgage { get; }
        public PrepayableMortgage Result { get; }

        public BuildTest()
        {
            var purchasedAt = new DateTime(2019, 1, 1);
            var downpayment = 82000.00M;
            var mortgageAmount = 328000.00M;
            var mortgageAmountMoney = new Money(mortgageAmount, purchasedAt);
            var preferredInterestRate = 0.0319M;

            Home = new Home("first home", purchasedAt, downpayment);
            BaseMortgage = new FixedRateMortgage(Home, mortgageAmountMoney, preferredInterestRate, 300);
            Subject = new PrepayableMortgageBuilder(BaseMortgage);
            Result = Subject.Build();
        }

        [TestCase(2019, 1, 1, 0.00)]
        [TestCase(2019, 1, 15, 1584.39)]
        [TestCase(2019, 1, 31, 1584.39)]
        [TestCase(2019, 2, 1, 1584.39)]
        [TestCase(2019, 2, 2, 1584.40)]
        [TestCase(2019, 12, 31, 1584.39)]
        [TestCase(2020, 1, 1, 32800.00)]
        [TestCase(2020, 1, 2, 1584.40)]
        [TestCase(2020, 2, 1, 1584.40)]
        [TestCase(2020, 12, 31, 1584.40)]
        [TestCase(2021, 1, 1, 32800)]
        [TestCase(2021, 1, 2, 1584.40)]
        [TestCase(2026, 4, 1, 1584.40)]
        [TestCase(2026, 5, 1, 1584.39)]
        [TestCase(2026, 6, 1, 1302.56)]
        [TestCase(2026, 7, 1, 1302.56)]
        [TestCase(2026, 10, 1, 1302.56)]
        public void Test_GetLatestPayment(int year, int month, int day, decimal expected)
        {
            Assert.That(
                Result.GetMonthlyPayments(new DateTime(year, month, day))
                    .Select(payment => payment.Amount)
                    .Select(amount => amount.Value)
                    .Last(),
                Is.EqualTo(expected)
            );
        }
    }
}
