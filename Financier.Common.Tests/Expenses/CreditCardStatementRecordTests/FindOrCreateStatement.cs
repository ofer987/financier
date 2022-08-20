using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

using Financier.Common.Expenses.Models;

namespace Financier.Common.Tests.Expenses.CreditCardStatementRecordTests
{
    public class FindOrCreateStatement
    {
        public class Base
        {
            public static class Cards
            {
                public static class One
                {
                    public const int Identifier = 1;
                    public static Guid Id = Guid.NewGuid();
                    public const string Number = "1234";

                    public static class Statements
                    {
                        public static class One
                        {
                            public const int Identifier = 1;
                            public static Guid Id = Guid.NewGuid();
                            public static DateTime PostedAt = new DateTime(2017, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                        }

                        public static class Two
                        {
                            public const int Identifier = 2;
                            public static Guid Id = Guid.NewGuid();
                            public static DateTime PostedAt = new DateTime(2017, 2, 1, 0, 0, 0, DateTimeKind.Utc);
                        }
                    }
                }

                public static class Two
                {
                    public const int Identifier = 2;
                    public static Guid Id = Guid.NewGuid();
                    public const string Number = "5678";

                    public static class Statements
                    {
                        public static class One
                        {
                            public const int Identifier = 1;
                            public static Guid Id = Guid.NewGuid();
                            public static DateTime PostedAt = new DateTime(2017, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                        }

                        public static class Two
                        {
                            public const int Identifier = 2;
                            public static Guid Id = Guid.NewGuid();
                            public static DateTime PostedAt = new DateTime(2018, 3, 1, 0, 0, 0, DateTimeKind.Utc);
                        }
                    }
                }
            }

            public Account Owner { get; set; }
            public Card Card1 { get; set; }
            public Card Card2 { get; set; }

            public Dictionary<int, Card> AllCards = new Dictionary<int, Card>();

            public Dictionary<int, Statement> Card1_Statements { get; set; } = new Dictionary<int, Statement>();
            public Dictionary<int, Statement> Card2_Statements { get; set; } = new Dictionary<int, Statement>();

            public int CardIdentifier { get; set; }
            public DateTime PostedAt { get; set; }
            public Guid ExpectedStatementId { get; set; }

            public Base(int cardIdentifier, DateTime postedAt, Guid expectedStatementId)
            {
                Owner = Factories.NewAccount(FactoryData.Accounts.Dan.AccountName);

                Card1 = Factories.NewCard(Owner, Cards.One.Number);
                Card1.Id = Cards.One.Id;
                AllCards.Add(Cards.One.Identifier, Card1);

                Card2 = Factories.NewCard(Owner, Cards.Two.Number);
                Card2.Id = Cards.Two.Id;
                AllCards.Add(Cards.Two.Identifier, Card2);

                {
                    var statement = Factories.NewStatement(Card1, Cards.One.Statements.One.PostedAt);
                    statement.Id = Cards.One.Statements.One.Id;
                    Card1_Statements.Add(Cards.One.Statements.One.Identifier, statement);
                }
                {
                    var statement = Factories.NewStatement(Card1, Cards.One.Statements.Two.PostedAt);
                    statement.Id = Cards.One.Statements.Two.Id;
                    Card1_Statements.Add(Cards.One.Statements.Two.Identifier, statement);
                }
                {
                    var statement = Factories.NewStatement(Card2, Cards.Two.Statements.One.PostedAt);
                    statement.Id = Cards.Two.Statements.One.Id;
                    Card2_Statements.Add(Cards.Two.Statements.One.Identifier, statement);
                }
                {
                    var statement = Factories.NewStatement(Card2, Cards.Two.Statements.Two.PostedAt);
                    statement.Id = Cards.Two.Statements.Two.Id;
                    Card2_Statements.Add(Cards.Two.Statements.Two.Identifier, statement);
                }

                CardIdentifier = cardIdentifier;
                PostedAt = postedAt;
                ExpectedStatementId = expectedStatementId;
            }

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
                    db.Accounts.Add(Owner);
                    db.SaveChanges();

                    db.Cards.Add(Card1);
                    db.Cards.Add(Card2);
                    db.SaveChanges();

                    foreach (var statement in Card1_Statements.Concat(Card2_Statements))
                    {
                        db.Statements.Add(statement.Value);
                        db.SaveChanges();
                    }
                }
            }
        }

        [TestFixtureSource(typeof(PositiveTests), nameof(TestCaseFixtures))]
        public class PositiveTests : Base
        {
            public PositiveTests(int cardIdentifier, DateTime postedAt, Guid expectedStatementId) :
                base(cardIdentifier, postedAt, expectedStatementId)
            {
            }

            [Test]
            public void Test_Expenses_CreditCardStatementRecord_FindOrCreateStatement_Creates_New_Statement()
            {
                int previousStatementCount;
                using (var db = new Context())
                {
                    previousStatementCount = db.Statements.Count();
                }

                var createdStatement = new CreditCardStatementRecord().FindOrCreateStatement(AllCards[CardIdentifier].Id, PostedAt);

                Assert.That(createdStatement.Id, Is.Not.AnyOf(AllCards.Select(card => card.Value.Id)));

                int newStatementCount;
                using (var db = new Context())
                {
                    newStatementCount = db.Statements.Count();
                }

                Assert.That(newStatementCount, Is.EqualTo(previousStatementCount + 1));
            }

            // [Test]
            // public void Test_Expenses_CreditCardStatementRecord_FindOrCreateStatement_New_Record()
            // {
            //     var newStatementId = Guid.NewGuid();
            //
            //     int previousStatementCount;
            //     using (var db = new Context())
            //     {
            //         previousStatementCount = db.Statements.Count();
            //     }
            //
            //     Console.WriteLine($"PostedAt = {PostedAt}, cardId = {AllCards[CardIdentifier].Id}");
            //     var createdStatement = new CreditCardStatementRecord().FindOrCreateStatement(newStatementId, PostedAt, AllCards[CardIdentifier].Id);
            //
            //     int newStatementCount;
            //     using (var db = new Context())
            //     {
            //         newStatementCount = db.Statements.Count();
            //     }
            //
            //     Assert.That(newStatementCount, Is.EqualTo(previousStatementCount + 1));
            // }

            public static IEnumerable TestCaseFixtures
            {
                get
                {
                    // yield return new TestCaseData(1);
                    yield return new TestFixtureData(Base.Cards.One.Identifier, new DateTime(2015, 1, 1, 0, 0, 0, DateTimeKind.Utc), Guid.NewGuid());
                    yield return new TestFixtureData(Base.Cards.Two.Identifier, new DateTime(2015, 1, 1, 0, 0, 0, DateTimeKind.Utc), Guid.NewGuid());
                }
            }
        }

        [TestFixtureSource(typeof(NegativeTests), nameof(TestCaseFixtures))]
        public class NegativeTests : Base
        {
            public NegativeTests(int cardIdentifier, DateTime postedAt, Guid expectedStatementId) :
                base(cardIdentifier, postedAt, expectedStatementId)
            {
            }

            [Test]
            public void Test_Expenses_CreditCardStatementRecord_FindOrCreateStatement_Creates_Same_Statement()
            {
                var newStatementId = Guid.NewGuid();

                int previousStatementCount;
                using (var db = new Context())
                {
                    previousStatementCount = db.Statements.Count();
                }

                var createdStatement = new CreditCardStatementRecord().FindOrCreateStatement(AllCards[CardIdentifier].Id, PostedAt);

                Assert.That(createdStatement.Id, Is.EqualTo(ExpectedStatementId));

                int newStatementCount;
                using (var db = new Context())
                {
                    newStatementCount = db.Statements.Count();
                }

                Assert.That(newStatementCount, Is.EqualTo(previousStatementCount));
            }

            // [Test]
            // public void Test_Expenses_CreditCardStatementRecord_FindOrCreateStatement_New_Record()
            // {
            //     var newStatementId = Guid.NewGuid();
            //
            //     int previousStatementCount;
            //     using (var db = new Context())
            //     {
            //         previousStatementCount = db.Statements.Count();
            //     }
            //
            //     var createdStatement = new CreditCardStatementRecord().FindOrCreateStatement(newStatementId, PostedAt, AllCards[CardIdentifier].Id);
            //
            //     int newStatementCount;
            //     using (var db = new Context())
            //     {
            //         newStatementCount = db.Statements.Count();
            //     }
            //
            //     Assert.That(newStatementCount, Is.EqualTo(previousStatementCount));
            // }

            public static IEnumerable TestCaseFixtures
            {
                get
                {
                    // yield return new TestCaseData(1);
                    yield return new TestFixtureData(Base.Cards.One.Identifier, new DateTime(2017, 1, 1, 0, 0, 0, DateTimeKind.Utc), Base.Cards.One.Statements.One.Id);
                    yield return new TestFixtureData(Base.Cards.Two.Identifier, new DateTime(2018, 3, 1, 0, 0, 0, DateTimeKind.Utc), Base.Cards.Two.Statements.Two.Id);
                }
            }
        }
    }
}
