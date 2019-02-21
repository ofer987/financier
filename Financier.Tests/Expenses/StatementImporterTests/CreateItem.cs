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
    public static class Cards
    {
        public static class One
        {
            public static Guid Id = Guid.NewGuid();
            public const string Number = "1234";

            public static class Statements
            {
                public static class One
                {
                    public static Guid Id = Guid.NewGuid();
                    public static DateTime PostedAt = new DateTime(2017, 1, 1);

                    public static class Items
                    {
                        public static class One
                        {
                            public static Guid Id = Guid.NewGuid();
                            public const string ItemId = "1";
                            public static decimal Amount = 200000.00M;
                            public static string Description = "Porsche 911";
                            public static DateTime TransactedAt = new DateTime(2025, 6, 5);
                            public static DateTime PostedAt = new DateTime(2025, 6, 5);
                        }
                    }
                }

                public static class Two
                {
                    public static Guid Id = Guid.NewGuid();
                    public static DateTime PostedAt = new DateTime(2017, 2, 1);
                }
            }
        }
    }

    public class CreateItem
    {
        public Card Card1 { get; set; }

        public Dictionary<Guid, Card> AllCards = new Dictionary<Guid, Card>();

        public Dictionary<Guid, Statement> Card1_Statements { get; set; } = new Dictionary<Guid, Statement>();

        public Func<string, StatementRecord> GivenStatementRecordFunc { get; set; }

        public void CleanProperties()
        {
            Card1 = null;
            AllCards = new Dictionary<Guid, Card>();
            Card1_Statements = new Dictionary<Guid, Statement>();
        }

        public void InitProperties()
        {
            Card1 = Fixtures.Cards.SimpleCard;
            Card1.Number = Cards.One.Number;
            AllCards.Add(Cards.One.Id, Card1);

            {
                var statement = Fixtures.Statements.GetSimpleStatement(Card1);
                statement.PostedAt = Cards.One.Statements.One.PostedAt;
                Card1_Statements.Add(Cards.One.Statements.One.Id, statement);

                {
                    var item = Fixtures.Items.ItemWithoutTags(statement);
                    item.ItemId = Cards.One.Statements.One.Items.One.ItemId;
                    item.Amount = Cards.One.Statements.One.Items.One.Amount;
                    item.Description = Cards.One.Statements.One.Items.One.Description;
                    item.PostedAt = Cards.One.Statements.One.Items.One.PostedAt;
                    item.TransactedAt = Cards.One.Statements.One.Items.One.TransactedAt;

                    statement.Items.Add(item);
                }
            }
            {
                var statement = Fixtures.Statements.GetSimpleStatement(Card1);
                statement.PostedAt = Cards.One.Statements.Two.PostedAt;
                Card1_Statements.Add(Cards.One.Statements.Two.Id, statement);
            }

            GivenStatementRecordFunc = (itemId) => 
            {
                return new StatementRecord
                {
                    Amount = "10.00",
                    CardNumber = "Q32",
                    Description = "fd",
                    ItemId = itemId,
                    PostedAt = "20190801",
                    TransactedAt = "20190801"
                };
            };
        }

        [OneTimeSetUp]
        public void InitAll()
        {
            Context.Environment = Environments.Test;
        }

        [SetUp]
        public void Init()
        {
            CleanProperties();
            InitProperties();

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
                db.Cards.Add(Card1);
                db.SaveChanges();

                foreach (var statement in Card1_Statements.Select(st => st.Value))
                {
                    db.Statements.Add(statement);
                    db.SaveChanges();
                }
            }
        }

        [Test]
        [TestCase(Cards.One.Statements.One.Items.One.ItemId + "2")]
        [TestCase("New_item_Id")]
        public void Test_Expenses_StatementImporter_CreateItem_Succeeds_If_Different_ItemId(string itemId)
        {
            Assert.DoesNotThrow(() => new StatementImporter().CreateItem(GivenStatementRecordFunc(itemId), Card1_Statements.Select(st => st.Value).First()));
        }

        [Test]
        [TestCase(Cards.One.Statements.One.Items.One.ItemId)]
        public void Test_Expenses_StatementImporter_CreateItem_Fails_If_Same_ItemId(string itemId)
        {
            int previousCount;
            using (var db = new Context())
            {
                previousCount = db.Items.Count();
            }
            Assert.Throws<DbUpdateException>(() => new StatementImporter().CreateItem(GivenStatementRecordFunc(itemId), Card1_Statements.Select(st => st.Value).First()));

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
            var statement = new Statement();

            Assert.Throws<ArgumentException>(() => new StatementImporter().CreateItem(GivenStatementRecordFunc(itemId), Card1_Statements.Select(st => st.Value).First()));
        }
    }
}
