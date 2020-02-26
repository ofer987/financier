using System;
using System.Linq;
using NUnit.Framework;

using Financier.Common.Models;
using Financier.Common.Expenses.Actions;

namespace Financier.Common.Tests.Expenses.ActionTests
{
    public class ActivityTest
    {
        public Product Television { get; private set; }
        public Product Stand { get; private set; }
        public Product House { get; private set; }
        public Activity Subject { get; private set; }
        public static DateTime InitiatedAt = new DateTime(2020, 1, 1);

        [SetUp]
        public void Init()
        {
            Television = new SimpleProduct("television", new Money(40.00M, InitiatedAt));
            Stand = new SimpleProduct("stand", new Money(20.00M, InitiatedAt));
            House = new SimpleProduct("stand", new Money(5000.00M, InitiatedAt));

            Subject = new Activity(InitiatedAt);
        }

        [Test]
        public void Test_GetHistory()
        {
            Subject.Buy(Television, new DateTime(2020, 1, 1));
            Subject.Sell(Television, new Money(50.00M, new DateTime(2020, 1, 1)), new DateTime(2020, 2, 1));
            Subject.Buy(Television, new DateTime(2020, 3, 1));
            Subject.Sell(Television, new Money(60.00M, new DateTime(2020, 1, 1)), new DateTime(2020, 4, 1));

            var actions = Subject.GetHistory(Television).ToList();
            Assert.That(actions[0].Product, Is.EqualTo(Television));
            Assert.That(actions[0].Type, Is.EqualTo(Types.Purchase));
            Assert.That(actions[0].Price, Is.EqualTo(new Money(40.00M, new DateTime(2020, 1, 1))));
            Assert.That(actions[1].Product, Is.EqualTo(Television));
            Assert.That(actions[1].Type, Is.EqualTo(Types.Sale));
            Assert.That(actions[1].Price, Is.EqualTo(new Money(50.00M, new DateTime(2020, 1, 1))));
            Assert.That(actions[2].Product, Is.EqualTo(Television));
            Assert.That(actions[2].Type, Is.EqualTo(Types.Purchase));
            Assert.That(actions[2].Price, Is.EqualTo(new Money(40.00M, new DateTime(2020, 1, 1))));
            Assert.That(actions[3].Product, Is.EqualTo(Television));
            Assert.That(actions[3].Type, Is.EqualTo(Types.Sale));
            Assert.That(actions[3].Price, Is.EqualTo(new Money(60.00M, new DateTime(2020, 1, 1))));
        }

        [Test]
        public void Test_Sell_CannotSellANonPurchasedProduct()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                Subject.Sell(Television, new Money(100.00M, new DateTime(2020, 1, 1)), new DateTime(2020, 5, 1));
            });
        }

        [Test]
        public void Test_Sell_CannotSellAProductTwice()
        {
            Subject.Buy(Television, new DateTime(2020, 1, 1));
            Subject.Sell(Television, new Money(100.00M, new DateTime(2020, 1, 1)), new DateTime(2020, 5, 1));

            Assert.Throws<InvalidOperationException>(() =>
            {
                Subject.Sell(Television, new Money(90.00M, new DateTime(2020, 1, 1)), new DateTime(2020, 6, 1));
            });
        }

        [Test]
        public void Test_Purchase_CannotPurchaseAProductTwice()
        {
            Subject.Buy(Television, new DateTime(2020, 1, 1));

            Assert.Throws<InvalidOperationException>(() =>
            {
                Subject.Buy(Television, new DateTime(2020, 2, 1));
            });
        }

        [Test]
        public void Test_Purchase_CannotPurchaseAProductBeforeItWasSold()
        {
            Subject.Buy(Television, new DateTime(2020, 1, 1));
            Subject.Sell(Television, new Money(90.00M, new DateTime(2020, 1, 1)), new DateTime(2020, 6, 1));

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                Subject.Buy(Television, new DateTime(2020, 5, 1));
            });
        }

        [Test]
        public void Test_Sell_CannotSellAProductBeforeItWasPurchased()
        {
            Subject.Buy(Television, new DateTime(2020, 1, 1));

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                Subject.Sell(Television, new Money(90.00M, new DateTime(2020, 1, 1)), new DateTime(2019, 12, 1));
            });
        }

        public void Test_Buy_CannotBuyBeforeInitiatedAt()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                Subject.Buy(Television, InitiatedAt.AddDays(-1));
            });
        }

        public void Test_Sell_CannotSellBeforeInitiatedAt()
        {
            Subject.Buy(Television, InitiatedAt);

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                Subject.Sell(Television, new Money(100.00M, InitiatedAt), InitiatedAt.AddDays(-1));
            });
        }

        [Test]
        public void Test_GetOwnedProducts()
        {
            Subject.Buy(Television, new DateTime(2020, 1, 1));
            Subject.Sell(Television, new Money(50.00M, new DateTime(2020, 2, 1)), new DateTime(2020, 2, 1));
            Subject.Buy(Television, new DateTime(2020, 3, 1));
            Subject.Sell(Television, new Money(60.00M, new DateTime(2020, 1, 1)), new DateTime(2020, 4, 1));
            Subject.Buy(Television, new DateTime(2020, 8, 1));

            Subject.Buy(Stand, new DateTime(2020, 3, 20));
            Subject.Sell(Stand, new Money(100.00M, new DateTime(2022, 2, 1)), new DateTime(2022, 2, 1));

            Subject.Buy(House, new DateTime(2022, 1, 1));

            Assert.That(
                Subject.GetOwnedProducts(new DateTime(2019, 12, 31)),
                Is.EquivalentTo(Enumerable.Empty<Product>())
            );
            Assert.That(
                Subject.GetOwnedProducts(new DateTime(2020, 1, 1)),
                Is.EquivalentTo(new[] { Television })
            );
            Assert.That(
                Subject.GetOwnedProducts(new DateTime(2020, 1, 2)),
                Is.EquivalentTo(new[] { Television })
            );
            Assert.That(
                Subject.GetOwnedProducts(new DateTime(2020, 2, 1)),
                Is.EquivalentTo(Enumerable.Empty<Product>())
            );
            Assert.That(
                Subject.GetOwnedProducts(new DateTime(2020, 2, 2)),
                Is.EquivalentTo(Enumerable.Empty<Product>())
            );
            Assert.That(
                Subject.GetOwnedProducts(new DateTime(2020, 3, 2)),
                Is.EquivalentTo(new[] { Television })
            );
            Assert.That(
                Subject.GetOwnedProducts(new DateTime(2020, 3, 21)),
                Is.EquivalentTo(new[] { Television, Stand })
            );
            Assert.That(
                Subject.GetOwnedProducts(new DateTime(2022, 1, 2)),
                Is.EquivalentTo(new[] { Television, Stand, House })
            );
        }
    }
}
