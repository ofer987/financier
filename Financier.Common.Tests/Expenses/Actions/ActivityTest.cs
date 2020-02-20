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

        [SetUp]
        public void Init()
        {
            Television = new SimpleProduct("television", new Money(40.00M, new DateTime(2020, 1, 1)));
            Stand = new SimpleProduct("stand", new Money(20.00M, new DateTime(2020, 1, 1)));
            House = new SimpleProduct("stand", new Money(5000.00M, new DateTime(2020, 1, 1)));
        }

        [Test]
        public void Test_GetHistory()
        {
            var activity = new Activity();

            activity.Buy(Television, new DateTime(2020, 1, 1));
            activity.Sell(Television, new Money(50.00M, new DateTime(2020, 1, 1)), new DateTime(2020, 2, 1));
            activity.Buy(Television, new DateTime(2020, 3, 1));
            activity.Sell(Television, new Money(60.00M, new DateTime(2020, 1, 1)), new DateTime(2020, 4, 1));

            var actions = activity.GetHistory(Television).ToList();
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
            var activity = new Activity();

            Assert.Throws<InvalidOperationException>(() =>
            {
                activity.Sell(Television, new Money(100.00M, new DateTime(2020, 1, 1)), new DateTime(2020, 5, 1));
            });
        }

        [Test]
        public void Test_Sell_CannotSellAProductTwice()
        {
            var activity = new Activity();
            activity.Buy(Television, new DateTime(2020, 1, 1));
            activity.Sell(Television, new Money(100.00M, new DateTime(2020, 1, 1)), new DateTime(2020, 5, 1));

            Assert.Throws<InvalidOperationException>(() =>
            {
                activity.Sell(Television, new Money(90.00M, new DateTime(2020, 1, 1)), new DateTime(2020, 6, 1));
            });
        }

        [Test]
        public void Test_Purchase_CannotPurchaseAProductTwice()
        {
            var activity = new Activity();
            activity.Buy(Television, new DateTime(2020, 1, 1));

            Assert.Throws<InvalidOperationException>(() =>
            {
                activity.Buy(Television, new DateTime(2020, 2, 1));
            });
        }

        [Test]
        public void Test_Purchase_CannotPurchaseAProductBeforeItWasSold()
        {
            var activity = new Activity();
            activity.Buy(Television, new DateTime(2020, 1, 1));
            activity.Sell(Television, new Money(90.00M, new DateTime(2020, 1, 1)), new DateTime(2020, 6, 1));

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                activity.Buy(Television, new DateTime(2020, 5, 1));
            });
        }

        [Test]
        public void Test_Sell_CannotSellAProductBeforeItWasPurchased()
        {
            var activity = new Activity();
            activity.Buy(Television, new DateTime(2020, 1, 1));

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                activity.Sell(Television, new Money(90.00M, new DateTime(2020, 1, 1)), new DateTime(2019, 12, 1));
            });
        }

        [Test]
        public void Test_GetOwnedProducts()
        {
            var activity = new Activity();

            activity.Buy(Television, new DateTime(2020, 1, 1));
            activity.Sell(Television, new Money(50.00M, new DateTime(2020, 2, 1)), new DateTime(2020, 2, 1));
            activity.Buy(Television, new DateTime(2020, 3, 1));
            activity.Sell(Television, new Money(60.00M, new DateTime(2020, 1, 1)), new DateTime(2020, 4, 1));
            activity.Buy(Television, new DateTime(2020, 8, 1));

            activity.Buy(Stand, new DateTime(2020, 3, 20));
            activity.Sell(Stand, new Money(100.00M, new DateTime(2022, 2, 1)), new DateTime(2022, 2, 1));

            activity.Buy(House, new DateTime(2022, 1, 1));

            Assert.That(
                activity.GetOwnedProducts(new DateTime(2019, 12, 31)),
                Is.EquivalentTo(Enumerable.Empty<Product>())
            );
            Assert.That(
                activity.GetOwnedProducts(new DateTime(2020, 1, 1)),
                Is.EquivalentTo(new[] { Television })
            );
            Assert.That(
                activity.GetOwnedProducts(new DateTime(2020, 1, 2)),
                Is.EquivalentTo(new[] { Television })
            );
            Assert.That(
                activity.GetOwnedProducts(new DateTime(2020, 2, 1)),
                Is.EquivalentTo(Enumerable.Empty<Product>())
            );
            Assert.That(
                activity.GetOwnedProducts(new DateTime(2020, 2, 2)),
                Is.EquivalentTo(Enumerable.Empty<Product>())
            );
            Assert.That(
                activity.GetOwnedProducts(new DateTime(2020, 3, 2)),
                Is.EquivalentTo(new[] { Television })
            );
            Assert.That(
                activity.GetOwnedProducts(new DateTime(2020, 3, 21)),
                Is.EquivalentTo(new[] { Television, Stand })
            );
            Assert.That(
                activity.GetOwnedProducts(new DateTime(2022, 1, 2)),
                Is.EquivalentTo(new[] { Television, Stand, House })
            );
        }
    }
}
