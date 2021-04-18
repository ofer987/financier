using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

using Financier.Common.Expenses;
using Financier.Common.Expenses.Models;

namespace Financier.Common.Tests.Expenses.TagManagerTests
{
    [TestFixtureSource(nameof(TestCaseFixtures))]
    public class GetSimilarTagsByDescription
    {
        public static class Descriptions
        {
            public const string OnlyDesc = "Only_desc";
            public const string Twice = "Twice";
            public const string Statement2Item = "statement2_item";
        }

        public Account Owner { get; set; }
        public List<Card> Cards { get; set; } = new List<Card>();
        public List<Statement> Statements { get; set; } = new List<Statement>();
        public List<Item> Items { get; set; } = new List<Item>();
        public Item NewItem { get; set; }

        public Tag DanTag1 { get; set; }
        public Tag RonTag1 { get; set; }
        public Tag KerenTag1 { get; set; }

        public string[] ExpectedTagNames { get; set; }
        public string Description { get; set; }

        public GetSimilarTagsByDescription(string description, string[] expectedTags)
        {
            Context.Environment = Environments.Test;
            Context.Clean();

            Owner = Factories.CreateAccount(FactoryData.Accounts.Dan.AccountName);

            var myCard1 = Factories.CreateCard(Owner, "1234");
            Cards.Add(myCard1);

            var myStatement1 = Factories.CreateSimpleStatement(myCard1, new DateTime(2018, 1, 1));
            var myStatement2 = Factories.CreateSimpleStatement(myCard1, new DateTime(2018, 2, 1));
            Statements.AddRange(new[] { myStatement1, myStatement2 });

            DanTag1 = Factories.DanTag();
            RonTag1 = Factories.RonTag();
            KerenTag1 = Factories.KerenTag();

            NewItem = Factories.CreateItemWithoutTags(
                myStatement1,
                "1234",
                string.Empty,
                new DateTime(2019, 1, 1),
                20.00M
            );
            NewItem.Description = description;

            {
                var item = Factories.CreateItemWithTags(
                    myStatement1,
                    "5678",
                    string.Empty,
                    new DateTime(2019, 1, 1),
                    20.00M,
                    new[] { DanTag1, RonTag1 }
                );
                item.Description = Descriptions.OnlyDesc;
                Items.Add(item);
            }
            {
                var item = Factories.CreateItemWithTags(
                    myStatement1,
                    "1357",
                    string.Empty,
                    new DateTime(2019, 1, 1),
                    20.00M,
                    new[] { DanTag1, RonTag1 }
                );
                item.Description = Descriptions.Twice;
                item.PostedAt = new DateTime(2018, 1, 1);
                Items.Add(item);
            }

            {
                var item = Factories.CreateItemWithTags(
                    myStatement2,
                    "4321",
                    string.Empty,
                    new DateTime(2019, 1, 1),
                    20.00M,
                    new[] { DanTag1, RonTag1, KerenTag1 }
                );
                item.Description = Descriptions.Twice;
                item.PostedAt = new DateTime(2018, 2, 1);
                Items.Add(item);
            }

            {
                var item = Factories.CreateItemWithTags(
                    myStatement2,
                    "9876", 
                    string.Empty,
                    new DateTime(2019, 1, 1),
                    20.00M,
                    new[] { DanTag1 }
                );
                item.Description = Descriptions.Statement2Item;
                item.PostedAt = new DateTime(2018, 1, 1);
                Items.Add(item);
            }

            {
                var item = Factories.CreateItemWithoutTags(
                    myStatement2,
                    "7531",
                    string.Empty,
                    new DateTime(2019, 1, 1),
                    20.00M
                );
                item.Description = Descriptions.Statement2Item;
                item.PostedAt = new DateTime(2018, 1, 3);
                Items.Add(item);
            }

            ExpectedTagNames = expectedTags;
            Description = description;
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
            InitDb1();
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            Context.Clean();
        }

        [Test]
        public void Test_Expenses_TagManager_GetSimilarTagsByDescription()
        {
            var actualTags = new TagManager(NewItem).GetSimilarTagsByDescription();

            Assert.That(actualTags.Select(t => t.Name), Is.EqualTo(ExpectedTagNames));
        }

        private void InitDb1()
        {
            using (var db = new Context())
            {
                db.Accounts.Add(Owner);
                db.SaveChanges();

                db.Tags.Add(DanTag1);
                db.Tags.Add(RonTag1);
                db.Tags.Add(KerenTag1);
                db.SaveChanges();

                foreach (var card in Cards)
                {
                    db.Cards.Add(card);
                    db.SaveChanges();
                }

                foreach (var statement in Statements)
                {
                    db.Statements.Add(statement);
                    db.SaveChanges();
                }

                foreach (var item in Items)
                {
                    db.Items.Add(item);
                    db.SaveChanges();
                }

                db.Items.Add(NewItem);
                db.SaveChanges();
            }
        }

        public static IEnumerable TestCaseFixtures
        {
            get
            {
                yield return new TestFixtureData(
                    Descriptions.OnlyDesc,
                    new[] {"dan", "ron"}
                );
                yield return new TestFixtureData(
                    "new_description",
                    new string[0]
                );
                yield return new TestFixtureData(
                    Descriptions.Twice,
                    new[] {"dan", "ron", "keren"}
                );
                yield return new TestFixtureData(
                    Descriptions.Statement2Item,
                    new[] {"dan"}
                );
                yield return new TestFixtureData(
                    string.Empty,
                    new string[0]
                );
            }
        }
    }
}
