using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

using Financier.Common.Expenses;
using Financier.Common.Expenses.Models;

namespace Financier.Common.Tests.Expenses.TagManagerTests
{
    [TestFixtureSource(typeof(AddTags), nameof(TestCaseFixtures))]
    public class AddTags
    {
        public Account Owner { get; set; }
        public Card MyCard1 { get; set; }
        public Statement MyStatement1 { get; set; }
        public Item MyItem1 { get; set; }
        public Tag DanTag1 { get; set; }
        public Tag RonTag1 { get; set; }
        public Tag KerenTag1 { get; set; }
        public List<Tag> ExistingTags { get; set; }
        public List<Tag> AddedTags { get; set; }
        public List<Tag> ExpectedTags { get; set; }

        public AddTags(string[] existingTags, string[] addedTags, string[] expectedTags)
        {
            Context.Environment = Environments.Test;
            Context.Clean();

            Owner = Factories.NewAccount("mrbean");
            MyCard1 = Factories.NewCard(Owner, "1234");
            MyStatement1 = Factories.NewStatement(MyCard1, new DateTime(2015, 1, 1));

            DanTag1 = Factories.DanTag();
            RonTag1 = Factories.RonTag();
            KerenTag1 = Factories.KerenTag();

            var allTags = new Dictionary<string, Tag>
            {
                { "dan", DanTag1 },
                { "ron", RonTag1 },
                { "keren", KerenTag1 }
            };

            ExistingTags = allTags
                .Where(pair => existingTags.Any(at => at == pair.Key))
                .Select(pair => pair.Value)
                .ToList();

            AddedTags = allTags
                .Where(pair => addedTags.Any(at => at == pair.Key))
                .Select(pair => pair.Value)
                .ToList();

            ExpectedTags = allTags
                .Where(pair => expectedTags.Any(at => at == pair.Key))
                .Select(pair => pair.Value)
                .ToList();

            MyItem1 = Factories.NewItem(
                MyStatement1,
                "1234",
                string.Empty,
                new DateTime(2019, 1, 1),
                20.00M
                // TODO Fix Me!!!!
                // ExistingTags
            );
        }

        [OneTimeSetUp]
        public void InitAll()
        {
            Context.Environment = Environments.Test;
        }

        [OneTimeSetUp]
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

        [Test]
        public void Test_Expenses_TagManager_AddTags()
        {
            var actualTags = new TagManager(MyItem1).AddTags(AddedTags).ToList();

            Assert.That(actualTags, Is.EqualTo(ExpectedTags));
        }

        private void InitDb()
        {
            using (var db = new Context())
            {
                db.Accounts.Add(Owner);
                db.SaveChanges();

                db.Tags.Add(DanTag1);
                db.Tags.Add(RonTag1);
                db.Tags.Add(KerenTag1);
                db.SaveChanges();

                db.Cards.Add(MyCard1);
                db.SaveChanges();

                db.Statements.Add(MyStatement1);
                db.SaveChanges();

                db.Items.Add(MyItem1);
                db.SaveChanges();
                MyItem1.AddTags(ExistingTags);
            }
        }

        public static IEnumerable TestCaseFixtures
        {
            get
            {
                yield return new TestFixtureData(
                    new string[] { },
                    new[] { "dan" },
                    new[] { "dan" }
                );
                yield return new TestFixtureData(
                    new string[] { },
                    new[] { "dan", "ron" },
                    new[] { "dan", "ron" }
                );
                yield return new TestFixtureData(
                    new string[] { "dan", "ron" },
                    new[] { "dan" },
                    new[] { "dan", "ron" }
                );
                yield return new TestFixtureData(
                    new string[] { "dan", "ron" },
                    new[] { "dan", "ron" },
                    new[] { "dan", "ron" }
                );
                yield return new TestFixtureData(
                    new string[] { "dan", "ron" },
                    new[] { "keren" },
                    new[] { "dan", "ron", "keren" }
                );
                yield return new TestFixtureData(
                    new string[] { "dan", "ron" },
                    new[] { "dan", "ron", "keren" },
                    new[] { "dan", "ron", "keren" }
                );
            }
        }
    }
}
