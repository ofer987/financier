using System;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using Microsoft.EntityFrameworkCore;

using Financier.Common;
using Financier.Common.Expenses;
using Financier.Common.Expenses.Models;

namespace Financier.Tests.Expenses.StatementImporterTests
{
    public static class Values
    {
        public static Guid DanCardId { get; private set; }
        public static Func<string, Card> GetDanCard = (number) => 
        {
            DanCardId = Guid.NewGuid();
            return new Card
            {
                Id = DanCardId,
                Number = number,
                Statements = new List<Statement>()
            };
        };

        public static Guid JuneStatementId { get; private set; }
        public static Func<Statement> GetJuneStatement = () =>
        { 
            JuneStatementId = Guid.NewGuid();
            return new Statement
            {
                Id = JuneStatementId,
                CardId = DanCardId,
                Items = new List<Item>(),
                PostedAt = new DateTime(2025, 7, 1)
            };
        };

        public const string PorscheItemId = "1234";
        public static Func<string, Item> GetPorscheItem = (itemId) => new Item
        {
            Id = Guid.NewGuid(),
            Amount = 300000.00M,
            Description = "Porsche 911",
            ItemId = itemId,
            ItemTags = new List<ItemTag>(),
            PostedAt = new DateTime(2025, 6, 5),
            TransactedAt = new DateTime(2025, 6, 5),
        };

        public static Func<string, StatementRecord> GetStatementRecord = (itemId) => new StatementRecord
        {
            Amount = "10.00",
            CardNumber = "Q32",
            Description = "fd",
            ItemId = itemId,
            PostedAt = "20190801",
            TransactedAt = "20190801"
        };
    }

    public class CreateItem
    {
        [OneTimeSetUp]
        public void InitAll()
        {
            Context.Environment = Environments.Test;
        }

        [SetUp]
        public void Init()
        {
            Context.Clean();

            InitDb();
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            Context.Clean();
        }

        private void InitDb()
        {
            using (var db = new Context())
            {
                var danCard = Values.GetDanCard(Guid.NewGuid().ToString());
                db.Cards.Add(danCard);
                db.SaveChanges();
                
                var juneStatement = Values.GetJuneStatement();
                juneStatement.Items.Add(Values.GetPorscheItem(Values.PorscheItemId));

                Console.WriteLine($"CardId = ({Values.DanCardId})");
                Console.WriteLine($"DanCard.Id = ({danCard.Id})");
                Console.WriteLine($"StatementId = ({Values.JuneStatementId})");
                Console.WriteLine($"JuneStatement.Id = ({juneStatement.Id})");
                db.Statements.Add(juneStatement);
                db.SaveChanges();
            }
        }

        [Test]
        [TestCase(Values.PorscheItemId + "additionaldata")]
        [TestCase("New_item_Id")]
        public void Test_Expenses_StatementImporter_CreateItem_Succeeds_If_Different_ItemId(string itemId)
        {
            var statementId = Values.JuneStatementId;
            Assert.DoesNotThrow(() => new StatementImporter().CreateItem(Values.GetStatementRecord(itemId), statementId));
        }

        [Test]
        [TestCase(Values.PorscheItemId)]
        public void Test_Expenses_StatementImporter_CreateItem_Fails_If_Same_ItemId(string itemId)
        {
            var statementId = Values.JuneStatementId;

            int previousCount;
            using (var db = new Context())
            {
                previousCount = db.Items.Count();
            }
            Assert.Throws<DbUpdateException>(() => new StatementImporter().CreateItem(Values.GetStatementRecord(itemId), statementId));

            using (var db = new Context())
            {
                Assert.That(db.Items.Count(), Is.EqualTo(previousCount));
            }
        }

        [Test]
        [TestCase("")]
        [TestCase("     ")]
        public void Test_Expenses_StatementImporter_CreateItem_Fails_If_Invalid_ItemId(string itemId)
        {
            var statementId = Values.JuneStatementId;
            Console.WriteLine("value is " + statementId);

            Assert.Throws<ArgumentException>(() => new StatementImporter().CreateItem(Values.GetStatementRecord(itemId), statementId));
        }
    }
}
