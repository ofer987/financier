using System.Linq;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

using Financier.Common;
using Financier.Common.Expenses;
using Financier.Common.Expenses.Models;

namespace Financier.Tests.Expenses.TagManagerTests
{
    [TestFixtureSource(typeof(AddTags), nameof(TestCaseFixtures))]
    public class AddTags
    {
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

            MyCard1 = Fixtures.Cards.SimpleCard;
            MyStatement1 = Fixtures.Statements.GetSimpleStatement(MyCard1);

            DanTag1 = Fixtures.Tags.DanTag();
            RonTag1 = Fixtures.Tags.RonTag();
            KerenTag1 = Fixtures.Tags.KerenTag();

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

            MyItem1 = Fixtures.Items.ItemWithTags(MyStatement1, ExistingTags);
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
        public void Test_Expenses_TagManager_AddTags()
        {
            var actualTags = new TagManager(MyItem1).AddTags(AddedTags).ToList();

            Assert.That(actualTags, Is.EqualTo(ExpectedTags));
        }

        private void InitDb1()
        {
            using (var db = new Context())
            {
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
            }
        }

        public static IEnumerable TestCaseFixtures
        {
            get
            {
                yield return new TestFixtureData(
                    new string[] {},
                    new[] {"dan"},
                    new[] {"dan"}
                );
                yield return new TestFixtureData(
                    new string[] {},
                    new[] {"dan", "ron"},
                    new[] {"dan", "ron"}
                );
                yield return new TestFixtureData(
                    new string[] {"dan", "ron"},
                    new[] {"dan"},
                    new[] {"dan", "ron"}
                );
                yield return new TestFixtureData(
                    new string[] {"dan", "ron"},
                    new[] {"dan", "ron"},
                    new[] {"dan", "ron"}
                );
                yield return new TestFixtureData(
                    new string[] {"dan", "ron"},
                    new[] {"keren"},
                    new[] {"dan", "ron", "keren"}
                );
                yield return new TestFixtureData(
                    new string[] {"dan", "ron"},
                    new[] {"dan", "ron", "keren"},
                    new[] {"dan", "ron", "keren"}
                );
            }
        }
    }
}
