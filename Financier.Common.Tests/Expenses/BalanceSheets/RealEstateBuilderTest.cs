using System;
using System.Linq;
using NUnit.Framework;

using Financier.Common.Expenses;
using Financier.Common.Extensions;
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
        public void Test_AddCondoWithFixedRateMortgage_ReturnsBuilder()
        {
            var purchasedAt = new DateTime(2019, 1, 2);
            var purchasePrice = new Money(2000.00M, purchasedAt);

            Assert.That(
                Subject
                    .SetInitialCash(purchasePrice)
                    .AddCondoWithFixedRateMortgage(purchasedAt, purchasePrice),
                Is.TypeOf<RealEstateBuilder>()
            );
        }

        [Test]
        public void Test_AddCondoWithFixedRateMortgage_HasHomeAndMortgage()
        {
            var purchasedAt = new DateTime(2019, 1, 2);
            var purchasePrice = new Money(2000.00M, purchasedAt);

            var balanceSheet = Subject
                .SetInitialCash(purchasePrice)
                .AddCondoWithFixedRateMortgage(purchasedAt, purchasePrice)
                .Build();

            Assert.That(
                balanceSheet.GetOwnedProducts(new DateTime(2019, 1, 2)).Count(),
                Is.EqualTo(2)
            );
        }

        [TestCase(2019, 1, 2, 100.00, 100.00)]
        [TestCase(2020, 1, 2, 100.00, 105.00)]
        [TestCase(2021, 1, 2, 100.00, 110.25)]
        [TestCase(2021, 2, 2, 100.00, 110.25)]
        [TestCase(2020, 1, 2, 200.00, 210.00)]
        public void Test_AddCondoWithFixedRateMortgage_FuturePurchasePriceIsInflated(int year, int month, int day, decimal purchasePriceAtInitiation, decimal expected)
        {
            var purchasedAt = new DateTime(year, month, day);
            var purchasePrice = purchasePriceAtInitiation;

            var balanceSheet = Subject
                .SetInitialCash(new Money(purchasePrice, purchasedAt))
                .AddCondoWithFixedRateMortgage(purchasedAt, purchasePriceAtInitiation)
                .Build();

            var condo = balanceSheet.GetOwnedProducts(purchasedAt).First();
            var actualAbsoluteHomePrice = condo.Price.Value;
            Assert.That(actualAbsoluteHomePrice, Is.EqualTo(expected));
        }
    }
}
