using System;
using NUnit.Framework;

using Financier.Common.Expenses;
using Financier.Common.Expenses.BalanceSheets;
using Financier.Common.Models;

namespace Financier.Common.Tests.Expenses.BalanceSheets
{
    // TODO:Rename this file and others to *Tests
    public class RealEstateBuilderTest
    {
        public DateTime InitiatedAt { get; private set; }
        public ICashFlow CashFlow { get; private set; }
        public RealEstateBuilder Subject { get; private set; }

        [SetUp]
        public void Init()
        {
            InitiatedAt = new DateTime(2019, 1, 1);
            CashFlow = new DummyCashFlow(100.00M);
            Subject = new RealEstateBuilder(CashFlow, InitiatedAt);
        }

        [Test]
        public void Test_AddHomeWithFixedRateMortgage_ReturnsBuilder()
        {
            var purchasedAt = new DateTime(2019, 1, 1);
            var purchasePrice = new Money(2000.00M, purchasedAt);

            Assert.That(
                Subject.AddHomeWithFixedRateMortgage(purchasedAt, purchasePrice),
                Is.TypeOf<RealEstateBuilder>()
            );
        }

        [Test]
        public void Test_AddHomeWithFixedRateMortgage_HasHome()
        {
            var purchasedAt = new DateTime(2019, 1, 1);
            var purchasePrice = new Money(2000.00M, purchasedAt);

            var balanceSheet = Subject
                .AddHomeWithFixedRateMortgage(purchasedAt, purchasePrice)
                .Build();

            Assert.That(balanceSheet.Homes.Count, Is.EqualTo(1));
        }

        [TestCase(2019, 1, 1, 100.00, 100.00)]
        [TestCase(2020, 1, 1, 100.00, 105.00)]
        [TestCase(2021, 1, 1, 100.00, 110.25)]
        [TestCase(2021, 2, 1, 100.00, 110.25)]
        [TestCase(2020, 1, 1, 200.00, 210.00)]
        public void Test_AddHomeWithFixedRateMortgage_FuturePurchasePriceIsInflated(int year, int month, int day, decimal purchasePriceAtInitiation, decimal expected)
        {
            var purchasedAt = new DateTime(year, month, day);
            var purchasePrice = purchasePriceAtInitiation;

            var balanceSheet = Subject
                .AddHomeWithFixedRateMortgage(purchasedAt, purchasePriceAtInitiation)
                .Build();

            var actualAbsoluteHomePrice = balanceSheet.Homes[0].Price.Value;
            Assert.That(actualAbsoluteHomePrice, Is.EqualTo(expected));
        }
    }
}
