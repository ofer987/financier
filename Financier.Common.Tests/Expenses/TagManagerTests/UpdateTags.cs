using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Microsoft.EntityFrameworkCore;

using Financier.Common.Expenses;
using Financier.Common.Expenses.Models;

namespace Financier.Common.Tests.Expenses.TagManagerTests
{
    [TestFixtureSource(typeof(UpdateTags), nameof(TestCaseFixtures))]
    public class UpdateTags
    {
        public Account OwnerOfMyCard1 { get; set; }
        public Card MyCard1 { get; set; }
        public Statement MyStatement1 { get; set; }
        public Item MyItem1 { get; set; }
        public Tag DanTag1 { get; set; }
        public Tag RonTag1 { get; set; }
        public Tag KerenTag1 { get; set; }
        public List<Tag> ExistingTags { get; set; }
        public List<Tag> UpdatedTags { get; set; }
        public List<Tag> UnusedTags { get; set; }

        public UpdateTags(string[] existingTags, string[] updatedTags, string[] unusedTags)
        {
            OwnerOfMyCard1 = ModelFactories.Accounts.GetMrBean();
            MyCard1 = Factories.SimpleCard(OwnerOfMyCard1);
            MyCard1.Number = Guid.NewGuid().ToString();
            MyStatement1 = Factories.GetSimpleStatement(MyCard1);

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

            UpdatedTags = allTags
                .Where(pair => updatedTags.Any(at => at == pair.Key))
                .Select(pair => pair.Value)
                .ToList();

            UnusedTags = allTags
                .Where(pair => unusedTags.Any(at => at == pair.Key))
                .Select(pair => pair.Value)
                .ToList();

            MyItem1 = Factories.ItemWithTags(MyStatement1, ExistingTags);
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
            InitDb1();
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            Context.Clean();
        }

        [Test]
        public void Test_Expenses_TagManager_UpdateTags_DoesNotThrowException()
        {
            Assert.DoesNotThrow(() =>
            {
                new TagManager(MyItem1).UpdateTags(UpdatedTags);
            });
        }

        [Test]
        public void Test_Expenses_TagManager_UpdateTags_DeletesUnusedItemTags()
        {
            new TagManager(MyItem1).UpdateTags(UpdatedTags);

            foreach (var unusedTag in UnusedTags)
            {
                using (var db = new Context())
                {
                    var exists = db.ItemTags
                        .Include(it => it.Tag)
                        .Any(it => it.Tag.Name == unusedTag.Name);

                    Assert.That(exists, Is.False);
                }
            }
        }

        [Test]
        public void Test_Expenses_TagManager_UpdateTags_CreatesNewTags()
        {
            new TagManager(MyItem1).UpdateTags(UpdatedTags);

            using (var db = new Context())
            {
                var actualTags = db.ItemTags
                    .Include(it => it.Tag)
                    .Include(it => it.Item)
                    .Where(it => it.Item.Id == MyItem1.Id)
                    .Select(it => it.Tag);

                Assert.That(actualTags.Select(tag => tag.Name), Is.EqualTo(UpdatedTags.Select(tag => tag.Name)));
            }
        }

        private void InitDb1()
        {
            using (var db = new Context())
            {
                db.Accounts.Add(OwnerOfMyCard1);
                db.SaveChanges();

                db.Tags.Add(DanTag1);
                db.Tags.Add(RonTag1);
                db.Tags.Add(KerenTag1);
                db.SaveChanges();

                Console.WriteLine($"We have {db.Cards.Count()} cards");
                db.Cards.Add(MyCard1);
                db.SaveChanges();

                Console.WriteLine($"{nameof(MyStatement1.Id)} = {MyStatement1.Id}");
                Console.WriteLine($"We have {db.Cards.Count()} cards");
                Console.WriteLine($"We have {db.Statements.Count()} statements");
                db.Statements.Add(MyStatement1);
                db.SaveChanges();

                db.Items.Add(MyItem1);
                db.SaveChanges();
            }
        }

        public static IEnumerable TestCaseFixtures
        {
            get
            {
                yield return new TestFixtureData(
                    new string[] { },
                    new[] { "dan" },
                    new string[] { }
                );
                yield return new TestFixtureData(
                    new string[] { },
                    new[] { "dan", "ron" },
                    new string[] { }
                );
                yield return new TestFixtureData(
                    new[] { "dan", "ron" },
                    new string[] { },
                    new string[] { "dan", "ron" }
                );
                yield return new TestFixtureData(
                    new string[] { "dan", "ron" },
                    new[] { "dan" },
                    new[] { "ron" }
                );
                yield return new TestFixtureData(
                    new string[] { "dan", "ron" },
                    new[] { "dan", "ron" },
                    new string[] { }
                );
                yield return new TestFixtureData(
                    new string[] { "dan", "ron" },
                    new[] { "keren" },
                    new[] { "dan", "ron" }
                );
                yield return new TestFixtureData(
                    new string[] { "dan", "ron" },
                    new[] { "dan", "ron", "keren" },
                    new string[] { }
                );
                yield return new TestFixtureData(
                    new string[] { "dan", "ron" },
                    new[] { "dan", "keren" },
                    new[] { "ron" }
                );
            }
        }
    }
}
