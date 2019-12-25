using System;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using Microsoft.EntityFrameworkCore;

using Financier.Common.Expenses.Models;

namespace Financier.Common.Tests.Expenses.CreditCardStatementRecordTests
{
    public class MyFactories : Factories
    {
        public static Guid DanCardId = Guid.NewGuid();
        public const string DanCardNumber = "1234567";
        public static Func<Card> GetDanCard = () => new Card
        {
            Id = DanCardId,
            Number = DanCardNumber,
            Statements = new List<Statement>()
        };

        public static Guid JuneStatementId = Guid.NewGuid();
        public static Func<Statement> GetJuneStatement = () => new Statement
        {
            Id = JuneStatementId,
            CardId = DanCardId,
            Items = new List<Item>(),
            PostedAt = new DateTime(2025, 7, 1)
        };

        public const string PorscheItemId = "1234";
        public static Guid PorscheId = Guid.NewGuid();
        public static Func<string, Item> GetPorscheItem = (itemId) => new Item
        {
            Id = PorscheId,
            Amount = 300000.00M,
            Description = "Porsche 911",
            ItemId = itemId,
            ItemTags = new List<ItemTag>(),
            PostedAt = new DateTime(2025, 6, 5),
            TransactedAt = new DateTime(2025, 6, 5),
        };

        public static Func<string, CreditCardStatementRecord> GetStatementRecord = (itemId) => new CreditCardStatementRecord
        {
            Amount = "10.00",
            Number = "Q32",
            Description = "fd",
            ItemId = itemId,
            PostedAt = "20190801",
            TransactedAt = "20190801"
        };
    }

    public class CreateItem : Fixture
    {
        protected override void InitStorage()
        {
            using (var db = new Context())
            {
                var danCard = MyFactories.GetDanCard();
                db.Cards.Add(danCard);
                db.SaveChanges();

                var juneStatement = MyFactories.GetJuneStatement();
                juneStatement.Items.Add(MyFactories.GetPorscheItem(MyFactories.PorscheItemId));

                db.Statements.Add(juneStatement);
                db.SaveChanges();
            }
        }

        [Test]
        [TestCase(MyFactories.PorscheItemId + "additionaldata")]
        [TestCase("New_item_Id")]
        public void Test_Expenses_CreditCardStatementRecord_CreateItem_Succeeds_If_Different_ItemId(string itemId)
        {
            Assert.DoesNotThrow(() => MyFactories.GetStatementRecord(itemId).CreateItem(MyFactories.JuneStatementId));
        }

        [Test]
        [TestCase(MyFactories.PorscheItemId)]
        public void Test_Expenses_CreditCardStatementRecord_CreateItem_Fails_If_Same_ItemId(string itemId)
        {
            int previousCount;
            using (var db = new Context())
            {
                previousCount = db.Items.Count();
            }
            Assert.Throws<DbUpdateException>(() => MyFactories.GetStatementRecord(itemId).CreateItem(MyFactories.JuneStatementId));

            using (var db = new Context())
            {
                Assert.That(db.Items.Count(), Is.EqualTo(previousCount));
            }
        }

        [Test]
        [TestCase("")]
        [TestCase("     ")]
        public void Test_Expenses_CreditCardStatementRecord_CreateItem_Fails_If_Invalid_ItemId(string itemId)
        {
            Assert.Throws<ArgumentException>(() => MyFactories.GetStatementRecord(itemId).CreateItem(MyFactories.JuneStatementId));
        }
    }
}
