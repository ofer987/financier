using System;
using System.Linq;
using NUnit.Framework;

using Financier.Common.Expenses;
using Financier.Common.Liabilities;
using Financier.Common.Extensions;
using Financier.Common.Expenses.BalanceSheets;
using Financier.Common.Models;
using Financier.Common.Expenses.Actions;

namespace Financier.Common.Tests.Expenses.BalanceSheets
{
    // TODO:Rename this file and others to *Tests
    public class MagicEstateBuilderTest
    {
        public DateTime InitiatedAt { get; private set; }
        public ICashFlow CashFlow { get; private set; }
        public MagicEstateBuilder Subject { get; private set; }

        [SetUp]
        public void Init()
        {
            InitiatedAt = new DateTime(2019, 1, 1);
            CashFlow = new DummyCashFlow(100.00M);
            Subject = new MagicEstateBuilder(CashFlow, InitiatedAt);
        }

        [Test]
        public void Test_AddCondoWithFixedRateMortgage_ReturnsBuilder()
        {
            var purchasedAt = new DateTime(2019, 1, 2);
            var purchasePrice = new Money(2000.00M, purchasedAt);

            Assert.That(
                Subject
                    .SetInitialCash(purchasePrice)
                    .AddCondoWithFixedRateMortgageAtDownPaymentPercentage("first", purchasePrice, 0.10M),
                Is.TypeOf<MagicEstateBuilder>()
            );
        }

        [Test]
        public void Test_AddCondoWithFixedRateMortgage_HasHomeAndMortgage()
        {
            var purchasedAt = new DateTime(2019, 1, 2);
            var purchasePrice = new Money(2000.00M, purchasedAt);

            var balanceSheet = Subject
                .SetInitialCash(purchasePrice)
                .AddCondoWithFixedRateMortgageAtDownPaymentPercentage("first", purchasePrice, 0.10M)
                .Build();

            Assert.That(
                balanceSheet.GetOwnedProducts(),
                Has.Exactly(2).Items
            );
        }

        [Test]
        public void Test_SellCondo_SellsTheHomeAndTheMortgage()
        {
            var purchasePrice = new Money(2000.00M, InitiatedAt);

            // Buy condo
            var balanceSheet = Subject
                .SetInitialCash(purchasePrice)
                .AddCondoWithFixedRateMortgageAtDownPaymentPercentage("first", purchasePrice, 0.25M);

            var condo = Subject.GetHomes().First();
            var purchasedAt = condo.PurchasedAt;

            // Sell the condo
            var soldAt = purchasedAt.AddYears(5);
            var homeInflation = new CompoundYearlyInflation(0.05M);
            var soldPrice = purchasePrice.GetValueAt(homeInflation, soldAt).Value;
            Subject.SellHome(condo, soldAt, soldPrice);

            Assert.That(Subject.GetHomes(), Is.Empty);
            var activity = Subject
                .Build();

            var actualHistories = activity.GetHistories()
                .ToList();
            Assert.That(actualHistories, Has.Exactly(4).Items);

            Assert.That(actualHistories[0].Price, Is.EqualTo(-1500.00M));
            Assert.That(actualHistories[0].Type, Is.EqualTo(Types.Purchase));
            Assert.That(actualHistories[0].At, Is.EqualTo(InitiatedAt.GetNext()));
            Assert.That(actualHistories[0].Product, Is.TypeOf<FixedRateMortgage>());

            Assert.That(actualHistories[1].Price, Is.EqualTo(condo.Financing.GetBalance(soldAt)));
            Assert.That(actualHistories[1].Type, Is.EqualTo(Types.Sale));
            Assert.That(actualHistories[1].At, Is.EqualTo(soldAt));
            Assert.That(actualHistories[1].Product, Is.TypeOf<FixedRateMortgage>());

            Assert.That(actualHistories[2].Price, Is.EqualTo(2000.00M));
            Assert.That(actualHistories[2].Type, Is.EqualTo(Types.Purchase));
            Assert.That(actualHistories[2].At, Is.EqualTo(InitiatedAt.GetNext()));
            Assert.That(actualHistories[2].Product, Is.TypeOf<Home>());

            Assert.That(actualHistories[3].Price, Is.EqualTo(soldPrice));
            Assert.That(actualHistories[3].Type, Is.EqualTo(Types.Sale));
            Assert.That(actualHistories[3].At, Is.EqualTo(soldAt));
            Assert.That(actualHistories[3].Product, Is.TypeOf<Home>());
        }

        [TestCase(5)]
        [TestCase(0)]
        public void Test_SellCondo_BeforeHomeWasPurchased(int yearsBeforeHomeWasPurchased)
        {
            var purchasePrice = new Money(2000.00M, InitiatedAt);

            // Buy condo
            var balanceSheet = Subject
                .SetInitialCash(purchasePrice)
                .AddCondoWithFixedRateMortgageAtDownPaymentPercentage("first", purchasePrice, 0.25M);

            var condo = Subject.GetHomes().First();
            var purchasedAt = condo.PurchasedAt;

            // Sell the condo
            var soldAt = purchasedAt.AddYears(0 -yearsBeforeHomeWasPurchased);
            var homeInflation = new CompoundYearlyInflation(0.05M);

            Assert.Throws<ArgumentException>(() =>
            {
                Subject.SellHome("first", soldAt, new Money(5000.00M, soldAt));
            });
        }
    }
}
