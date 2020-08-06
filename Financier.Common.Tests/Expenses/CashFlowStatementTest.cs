using System;
using NUnit.Framework;

using Financier.Common.Liabilities;
using Financier.Common.Expenses;
using Financier.Common.Expenses.Actions;
using Financier.Common.Models;

namespace Financier.Common.Tests.Expenses
{
    // TODO:Rename this file and others to *Tests
    public class CashFlowStatementTest
    {
        public DateTime InitiatedAt => Subject.InitiatedAt;
        public ICashFlow CashFlow { get; private set; }
        public Activity Subject { get; private set; }
        public Home FirstHome { get; private set; }
        public Home SecondHome { get; private set; }

        [SetUp]
        public void Init()
        {
            var initiatedAt = new DateTime(2019, 1, 1);
            var downpayment = 82000.00M;
            var mortgageAmount = 328000.00M;
            var preferredInterestRate = 0.0319M;

            var initialCash = 10000.00M;
            var initialDebt = 5000.00M;

            CashFlow = new DummyCashFlow(89.86M);
            Subject = new Activity(initialCash, initialDebt, CashFlow, initiatedAt);

            {
                var purchasedAt = initiatedAt;
                var mortgage = new FixedRateMortgage(
                    mortgageAmount,
                    preferredInterestRate,
                    300,
                    purchasedAt
                );
                FirstHome = new Home(
                    "first home",
                    purchasedAt,
                    downpayment + mortgageAmount,
                    downpayment,
                    mortgage
                );

                Subject.Buy(FirstHome, purchasedAt);
                Subject.Buy(FirstHome.Financing, purchasedAt);
            }

            // Sell the first home
            {
                var soldAt = new DateTime(2020, 1, 3);
                Subject.Sell(FirstHome, 500000.00M, soldAt);
                var leftOverMortgageBalance = 0.00M - FirstHome.Financing.GetBalance(soldAt);
                Subject.Sell(
                    FirstHome.Financing,
                    leftOverMortgageBalance,
                    soldAt
                );
            }

            {
                var purchasedAt = new DateTime(2020, 2, 3);
                var mortgage = new FixedRateMortgage(
                    mortgageAmount,
                    preferredInterestRate,
                    300,
                    purchasedAt
                );
                SecondHome = new Home(
                    "second home",
                    purchasedAt,
                    downpayment + mortgageAmount,
                    downpayment,
                    mortgage
                );
                Subject.Buy(SecondHome, purchasedAt);
                Subject.Buy(mortgage, purchasedAt);
            }
        }

        [TestCase(2019, 1, 1, 2019, 1, 2, -82000 + 89.86 * 1 - (1000.00 + 8500 + 800))]
        [TestCase(2019, 1, 1, 2019, 1, 31, -82000 + 89.86 * 30 - (1000.00 + 8500 + 800))]
        [TestCase(2019, 1, 1, 2019, 2, 28, -82000 + 89.86 * 58 - (1000.00 + 8500 + 800))]
        [TestCase(2019, 2, 1, 2019, 2, 28, 0 + 89.86 * 27)]
        [TestCase(2019, 2, 1, 2020, 1, 4, 0 + 89.86 * 337 + 500000 - 318588.78 - (500000 * 0.05))]
        [TestCase(2019, 2, 1, 2020, 2, 3, 0 + 89.86 * 367 + 500000 - 318588.78 - (500000 * 0.05))]
        [TestCase(2019, 2, 1, 2020, 2, 4, 0 + 89.86 * 368 + 500000 - 318588.78 - 82000 - (1000.00 + 8500 + 800) - (500000 * 0.05))]
        [TestCase(2020, 2, 1, 2020, 2, 4, 89.86 * 3 - 82000 - (1000.00 + 8500 + 800))]
        public void Test_GetCash(int startYear, int startMonth, int startDay,
            int endYear, int endMonth, int endDay,
            decimal expected)
        {
            var startAt = new DateTime(startYear, startMonth, startDay);
            var endAt = new DateTime(endYear, endMonth, endDay);

            Assert.That(
                Subject.GetCash(
                    startAt,
                    endAt
                ),
                Is.EqualTo(expected)
            );
        }
    }
}
