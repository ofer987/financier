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
        public Activity Activity { get; private set; }
        public static DateTime InitiatedAt = new DateTime(2020, 1, 1);

        [SetUp]
        public void Init()
        {
            Television = new SimpleProduct("television", new Money(40.00M, InitiatedAt));
            Stand = new SimpleProduct("stand", new Money(20.00M, InitiatedAt));
            House = new SimpleProduct("stand", new Money(5000.00M, InitiatedAt));

            Activity = new Activity(InitiatedAt);
        }

        [Test]
        public void Test_GetHistory()
        {
            Activity.Buy(Television, new DateTime(2020, 1, 1));
            Activity.Sell(Television, new Money(50.00M, new DateTime(2020, 1, 1)), new DateTime(2020, 2, 1));
            Activity.Buy(Television, new DateTime(2020, 3, 1));
            Activity.Sell(Television, new Money(60.00M, new DateTime(2020, 1, 1)), new DateTime(2020, 4, 1));

            var actions = Activity.GetHistory(Television).ToList();
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
                Activity.Sell(Television, new Money(100.00M, new DateTime(2020, 1, 1)), new DateTime(2020, 5, 1));
            });
        }

        [Test]
        public void Test_Sell_CannotSellAProductTwice()
        {
            Activity.Buy(Television, new DateTime(2020, 1, 1));
            Activity.Sell(Television, new Money(100.00M, new DateTime(2020, 1, 1)), new DateTime(2020, 5, 1));

            Assert.Throws<InvalidOperationException>(() =>
            {
                Activity.Sell(Television, new Money(90.00M, new DateTime(2020, 1, 1)), new DateTime(2020, 6, 1));
            });
        }

        [Test]
        public void Test_Purchase_CannotPurchaseAProductTwice()
        {
            Activity.Buy(Television, new DateTime(2020, 1, 1));

            Assert.Throws<InvalidOperationException>(() =>
            {
                Activity.Buy(Television, new DateTime(2020, 2, 1));
            });
        }

        [Test]
        public void Test_Purchase_CannotPurchaseAProductBeforeItWasSold()
        {
            Activity.Buy(Television, new DateTime(2020, 1, 1));
            Activity.Sell(Television, new Money(90.00M, new DateTime(2020, 1, 1)), new DateTime(2020, 6, 1));

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                Activity.Buy(Television, new DateTime(2020, 5, 1));
            });
        }

        [Test]
        public void Test_Sell_CannotSellAProductBeforeItWasPurchased()
        {
            Activity.Buy(Television, new DateTime(2020, 1, 1));

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                Activity.Sell(Television, new Money(90.00M, new DateTime(2020, 1, 1)), new DateTime(2019, 12, 1));
            });
        }

        public void Test_Buy_CannotBuyBeforeInitiatedAt()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                Activity.Buy(Television, InitiatedAt.AddDays(-1));
            });
        }

        public void Test_Sell_CannotSellBeforeInitiatedAt()
        {
            Activity.Buy(Television, InitiatedAt);

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                Activity.Sell(Television, new Money(100.00M, InitiatedAt), InitiatedAt.AddDays(-1));
            });
        }

        [Test]
        public void Test_GetOwnedProducts()
        {
            Activity.Buy(Television, new DateTime(2020, 1, 1));
            Activity.Sell(Television, new Money(50.00M, new DateTime(2020, 2, 1)), new DateTime(2020, 2, 1));
            Activity.Buy(Television, new DateTime(2020, 3, 1));
            Activity.Sell(Television, new Money(60.00M, new DateTime(2020, 1, 1)), new DateTime(2020, 4, 1));
            Activity.Buy(Television, new DateTime(2020, 8, 1));

            Activity.Buy(Stand, new DateTime(2020, 3, 20));
            Activity.Sell(Stand, new Money(100.00M, new DateTime(2022, 2, 1)), new DateTime(2022, 2, 1));

            Activity.Buy(House, new DateTime(2022, 1, 1));

            Assert.That(
                Activity.GetOwnedProducts(new DateTime(2019, 12, 31)),
                Is.EquivalentTo(Enumerable.Empty<Product>())
            );
            Assert.That(
                Activity.GetOwnedProducts(new DateTime(2020, 1, 1)),
                Is.EquivalentTo(new[] { Television })
            );
            Assert.That(
                Activity.GetOwnedProducts(new DateTime(2020, 1, 2)),
                Is.EquivalentTo(new[] { Television })
            );
            Assert.That(
                Activity.GetOwnedProducts(new DateTime(2020, 2, 1)),
                Is.EquivalentTo(Enumerable.Empty<Product>())
            );
            Assert.That(
                Activity.GetOwnedProducts(new DateTime(2020, 2, 2)),
                Is.EquivalentTo(Enumerable.Empty<Product>())
            );
            Assert.That(
                Activity.GetOwnedProducts(new DateTime(2020, 3, 2)),
                Is.EquivalentTo(new[] { Television })
            );
            Assert.That(
                Activity.GetOwnedProducts(new DateTime(2020, 3, 21)),
                Is.EquivalentTo(new[] { Television, Stand })
            );
            Assert.That(
                Activity.GetOwnedProducts(new DateTime(2022, 1, 2)),
                Is.EquivalentTo(new[] { Television, Stand, House })
            );
        }
    }
}
