using System;
using NUnit.Framework;

using Financier.Common.Liabilities;
using Financier.Common.Expenses;
using Financier.Common.Models;

namespace Financier.Common.Tests.Expenses
{
    // TODO:Rename this file and others to *Tests
    public class BalanceSheetTest
    {
        public DateTime InitiatedAt => Subject.InitiatedAt;
        public ICashFlow CashFlow { get; private set; }
        public BalanceSheet Subject { get; private set; }

        [SetUp]
        public void Init()
        {
            var initiatedAt = new DateTime(2019, 1, 1);
            var downpayment = 82000.00M;
            var mortgageAmount = 328000.00M;
            var mortgageAmountMoney = new Money(mortgageAmount, initiatedAt);
            var preferredInterestRate = 0.0319M;

            var initialCash = new Money(10000.00M, initiatedAt);
            var initialDebt = new Money(5000.00M, initiatedAt);

            CashFlow = new DummyCashFlow(89.86M);
            Subject = new BalanceSheet(initialCash, initialDebt, CashFlow, initiatedAt);

            {
                var purchasedAt = initiatedAt;
                var mortgage = new FixedRateMortgage(
                    mortgageAmountMoney,
                    preferredInterestRate,
                    300,
                    purchasedAt
                );
                var home = new Home(
                    "first home",
                    purchasedAt,
                    new Money(downpayment + mortgageAmountMoney, purchasedAt),
                    new Money(downpayment, purchasedAt),
                    mortgage
                );
                Subject.Buy(home);
            }

            {
                var purchasedAt = new DateTime(2020, 2, 3);
                var mortgage = new FixedRateMortgage(
                    mortgageAmountMoney,
                    preferredInterestRate,
                    300,
                    purchasedAt
                );
                var home = new Home(
                    "second home",
                    purchasedAt,
                    new Money(downpayment + mortgageAmountMoney, purchasedAt),
                    new Money(downpayment, purchasedAt),
                    mortgage
                );
                Subject.Buy(home);
            }
        }

        [Test]
        public void Test_GetAssets_CannotGetValuesBeforeInitiation()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                Subject.GetAssets(
                    Inflations.GetInflation(InflationTypes.NoopInflation),
                    Subject.InitiatedAt.AddDays(-1)
                );
            });
        }

        [Test]
        public void Test_GetLiabilities_CannotGetValuesBeforeInitiation()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                Subject.GetLiabilities(
                    Inflations.GetInflation(InflationTypes.NoopInflation),
                    Subject.InitiatedAt.AddDays(-1)
                );
            });
        }

        [Test]
        public void Test_GetNetWorth_CannotGetValuesBeforeInitiation()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                Subject.GetNetWorth(
                    Inflations.GetInflation(InflationTypes.NoopInflation),
                    Subject.InitiatedAt.AddDays(-1)
                );
            });
        }

        [TestCase(InflationTypes.NoopInflation, 2019, 1, 1, 10000.00 + 89.86 * 0)]
        [TestCase(InflationTypes.NoopInflation, 2019, 1, 2, 10000.00 + 82000.00 + 712.46 + 89.86 * 1)]
        [TestCase(InflationTypes.NoopInflation, 2019, 1, 15, 10000.00 + 82000.00 + 712.46 + 89.86 * 14)]
        [TestCase(InflationTypes.NoopInflation, 2019, 2, 3, 10000.00 + 82000.00 + 712.46 + 714.36 + 89.86 * 33)]
        [TestCase(InflationTypes.NoopInflation, 2020, 2, 2, 10000.00 + 82000.00 + 10148.70 + 89.86 * (33 + 364))]
        [TestCase(InflationTypes.NoopInflation, 2020, 2, 3, 10000.00 + 82000.00 + 10148.70 + 89.86 * (34 + 364))]
        [TestCase(InflationTypes.NoopInflation, 2020, 2, 4, 10000.00 + 82000.00 + 10148.70 + 89.86 * (35 + 364) + 82000 + 712.46)]
        public void Test_GetAssets(InflationTypes inflationType, int year, int month, int day, decimal expected)
        {
            var inflation = Inflations.GetInflation(inflationType);

            Assert.That(
                Subject.GetAssets(inflation, new DateTime(year, month, day)),
                Is.EqualTo(expected)
            );
        }

        [TestCase(InflationTypes.NoopInflation, 2019, 1, 1, 5000)]
        [TestCase(InflationTypes.NoopInflation, 2019, 1, 2, 5000 + 327287.54)]
        [TestCase(InflationTypes.NoopInflation, 2019, 1, 15, 5000 + 327287.54)]
        [TestCase(InflationTypes.NoopInflation, 2019, 2, 3, 5000 + 326573.18)]
        [TestCase(InflationTypes.NoopInflation, 2020, 2, 2, 5000 + 317851.30)]
        [TestCase(InflationTypes.NoopInflation, 2020, 2, 3, 5000 + 317851.30)]
        [TestCase(InflationTypes.NoopInflation, 2020, 2, 4, 5000 + 317851.30 + 327287.54)]
        public void Test_GetLiabilities(InflationTypes inflationType, int year, int month, int day, decimal expected)
        {
            var inflation = Inflations.GetInflation(inflationType);
            Assert.That(
                Subject.GetLiabilities(inflation, new DateTime(year, month, day)),
                Is.EqualTo(expected)
            );
        }

        [TestCase(InflationTypes.NoopInflation, 2019, 1, 1, 10000.00 + 89.86 * 0 - 5000)]
        [TestCase(InflationTypes.NoopInflation, 2020, 2, 4, 10000.00 + 82000.00 + 10148.70 + 89.86 * (35 + 364) + 82000 + 712.46 - 5000 - 317851.30 - 327287.54)]
        public void Test_GetNetWorth(InflationTypes inflationType, int year, int month, int day, decimal expected)
        {
            var inflation = Inflations.GetInflation(inflationType);

            Assert.That(
                Subject.GetNetWorth(inflation, new DateTime(year, month, day)),
                Is.EqualTo(expected)
            );
        }

        [TestCase(1)]
        [TestCase(2)]
        public void Test_AddHome_CannotAddHomeBeforeInitiation(int daysBeforeInitiation)
        {
            var purchasedAt = InitiatedAt.AddDays(0 - daysBeforeInitiation);
            var mortgageAmount = 328000.00M;
            var mortgageAmountMoney = new Money(mortgageAmount, purchasedAt);
            var preferredInterestRate = 0.0319M;
            var downPayment = 10000.00M;
            var mortgage = new FixedRateMortgage(
                mortgageAmountMoney,
                preferredInterestRate,
                300,
                purchasedAt
            );
            var home = new Home(
                "first home",
                purchasedAt,
                new Money(downPayment + mortgageAmountMoney, purchasedAt),
                new Money(downPayment, purchasedAt),
                mortgage
            );

            Assert.Throws<ArgumentOutOfRangeException>(() => Subject.Buy(home));
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        public void Test_AddHome_AfterInitiation_Succeeds(int daysAfterInitiation)
        {
            var purchasedAt = InitiatedAt.AddDays(daysAfterInitiation);
            var mortgageAmount = 328000.00M;
            var mortgageAmountMoney = new Money(mortgageAmount, purchasedAt);
            var preferredInterestRate = 0.0319M;
            var downPayment = 10000.00M;
            var mortgage = new FixedRateMortgage(
                mortgageAmountMoney,
                preferredInterestRate,
                300,
                purchasedAt
            );
            var home = new Home(
                "first home",
                purchasedAt,
                new Money(downPayment + mortgageAmountMoney, purchasedAt),
                new Money(downPayment, purchasedAt),
                mortgage
            );

            Assert.DoesNotThrow(() => Subject.Buy(home));
        }
    }
}
