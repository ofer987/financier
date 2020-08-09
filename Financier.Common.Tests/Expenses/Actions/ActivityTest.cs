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
        public DateTime InitiatedAt { get; private set; }

        [SetUp]
        public void Init()
        {
            Television = new SimpleProduct("television", 40.00M);
            Stand = new SimpleProduct("stand", 20.00M);
            House = new SimpleProduct("stand", 5000.00M);
            InitiatedAt = new DateTime(2019, 1, 1);

            Subject = new Activity(InitiatedAt);
        }

        [Test]
        public void Test_GetHistory()
        {
            Subject.Buy(Television, new DateTime(2020, 1, 1));
            Subject.Sell(Television, 50.00M, new DateTime(2020, 2, 1));
            Subject.Buy(Television, new DateTime(2020, 3, 1));
            Subject.Sell(Television, 60.00M, new DateTime(2020, 4, 1));

            var actions = Subject.GetHistory(Television).ToList();
            Assert.That(actions[0].Product, Is.EqualTo(Television));
            Assert.That(actions[0].Type, Is.EqualTo(Types.Purchase));
            Assert.That(actions[0].TransactionalPrice, Is.EqualTo(40.00M));
            Assert.That(actions[1].Product, Is.EqualTo(Television));
            Assert.That(actions[1].Type, Is.EqualTo(Types.Sale));
            Assert.That(actions[1].TransactionalPrice, Is.EqualTo(50.00M));
            Assert.That(actions[2].Product, Is.EqualTo(Television));
            Assert.That(actions[2].Type, Is.EqualTo(Types.Purchase));
            Assert.That(actions[2].TransactionalPrice, Is.EqualTo(40.00M));
            Assert.That(actions[3].Product, Is.EqualTo(Television));
            Assert.That(actions[3].Type, Is.EqualTo(Types.Sale));
            Assert.That(actions[3].TransactionalPrice, Is.EqualTo(60.00M));
        }

        [Test]
        public void Test_Sell_CannotSellANonPurchasedProduct()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                Subject.Sell(Television, 100.00M, new DateTime(2020, 5, 1));
            });
        }

        [Test]
        public void Test_Sell_CannotSellAProductTwice()
        {
            Subject.Buy(Television, new DateTime(2020, 1, 1));
            Subject.Sell(Television, 100.00M, new DateTime(2020, 5, 1));

            Assert.Throws<InvalidOperationException>(() =>
            {
                Subject.Sell(Television, 90.00M, new DateTime(2020, 6, 1));
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
            Subject.Sell(Television, 90.00M, new DateTime(2020, 6, 1));

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
                Subject.Sell(Television, 90.00M, new DateTime(2019, 12, 1));
            });
        }

        [Test]
        public void Test_Buy_CannotBuyBeforeInitiatedAt()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                Subject.Buy(Television, InitiatedAt.AddDays(-1));
            });
        }

        [Test]
        public void Test_Sell_CannotSellBeforeInitiatedAt()
        {
            Subject.Buy(Television, InitiatedAt);

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                Subject.Sell(Television, 100.00M, InitiatedAt.AddDays(-1));
            });
        }

        [Test]
        public void Test_BeforeInitiatedAt_ThrowsException()
        {
            var inflation = Inflations.NoopInflation;
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                Subject.GetCashAt(inflation, InitiatedAt.AddDays(-1));
            });
        }

        [Test]
        public void Test_AtInitiatedAt_ThrowsException()
        {
            var inflation = Inflations.NoopInflation;
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                Subject.GetCashAt(inflation, InitiatedAt);
            });
        }

        [Test]
        public void Test_AfterInitiatedAt_DoesNotThrowException()
        {
            var inflation = Inflations.NoopInflation;
            Assert.DoesNotThrow(() =>
            {
                Subject.GetCashAt(inflation, InitiatedAt.AddDays(1));
            });
        }

        [Test]
        public void Test_GetOwnedProducts()
        {
            Subject.Buy(Television, new DateTime(2020, 1, 1));
            Subject.Sell(Television, 50.00M, new DateTime(2020, 2, 1));
            Subject.Buy(Television, new DateTime(2020, 3, 1));
            Subject.Sell(Television, 60.00M, new DateTime(2020, 4, 1));
            Subject.Buy(Television, new DateTime(2020, 8, 1));

            Subject.Buy(Stand, new DateTime(2020, 3, 20));
            Subject.Sell(Stand, 100.00M, new DateTime(2022, 2, 1));

            Subject.Buy(House, new DateTime(2022, 1, 1));

            Assert.That(
                Subject.GetOwnedProducts(new DateTime(2019, 12, 31)),
                Is.Empty
            );
            Assert.That(
                Subject.GetOwnedProducts(new DateTime(2020, 1, 1)),
                Is.Empty
            );
            Assert.That(
                Subject.GetOwnedProducts(new DateTime(2020, 1, 1).AddSeconds(1)),
                Is.EquivalentTo(new[] { Television })
            );
            Assert.That(
                Subject.GetOwnedProducts(new DateTime(2020, 1, 2)),
                Is.EquivalentTo(new[] { Television })
            );
            Assert.That(
                Subject.GetOwnedProducts(new DateTime(2020, 2, 1).AddMilliseconds(1)),
                Is.Empty
            );
            Assert.That(
                Subject.GetOwnedProducts(new DateTime(2020, 2, 2)),
                Is.Empty
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
