using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
// using Microsoft.Data.Sqlite;
// using Microsoft.EntityFrameworkCore;

using Financier.Common;
using Financier.Common.Models.Expenses;

namespace Financier.Tests
{
    [TestFixtureSource(typeof(TagManagerTests), nameof(TestCaseFixtures))]
    public class TagManagerTests
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

        public TagManagerTests(string[] existingTags, string[] addedTags, string[] expectedTags)
        {
            ExpensesContext.Environment = Environments.Test;
            ExpensesContext.Clean();

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
            ExpensesContext.Environment = Environments.Test;
        }

        [SetUp]
        public void Init()
        {
            ExpensesContext.Clean();
            InitDb1();
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            ExpensesContext.Clean();
        }

        public static IEnumerable TestCases
        {
            get
            {
                yield return new TestCaseData("hello, dan, ron").Returns(new [] 
                            {
                new Tag
                {
                    Id = Guid.NewGuid(),
                    Name = "hello"
                },
                new Tag
                {
                    Id = Guid.NewGuid(),
                    Name = "dan"
                },
                new Tag
                {
                    Id = Guid.NewGuid(),
                    Name = "ron"
                }
                            });
                yield return new TestCaseData(", hello  , dan,ron, ron  , ").Returns(new [] 
                        {
                        new Tag
                        {
                        Id = Guid.NewGuid(),
                        Name = "hello"
                        },
                        new Tag
                        {
                        Id = Guid.NewGuid(),
                        Name = "dan"
                        },
                        new Tag
                        {
                        Id = Guid.NewGuid(),
                        Name = "ron"
                        }
                        });
                yield return new TestCaseData(string.Empty).Returns(new Tag[] { });
                yield return new TestCaseData("  ,   ,    ,").Returns(new Tag[] { });
                yield return new TestCaseData("Dan, dan, DAN, dAN").Returns(new [] 
                        {
                        new Tag
                        {
                        Id = Guid.NewGuid(),
                        Name = "dan"
                        }
                        });
            }
        }

        [Test]
        [TestCaseSource(nameof(TestCases))]
        public List<Tag> Test_FindOrCreateTags(string list)
        {
            return TagManager.FindOrCreateTags(list);
        }

        [Test]
        // [TestCaseSource(nameof(TestCases_Tags))]
        public void Test_AddNewTags()
        {
            Console.WriteLine("foo");
            var actualTags = new TagManager(MyItem1).AddTags(AddedTags).ToList();

            // Actual Tags are
            Console.WriteLine("Actual Tags");
            foreach (var tag in actualTags)
            {
                Console.WriteLine($"\thas tag.Name ({tag.Name})");
            }

            Console.WriteLine("MyItem1.Tags");
            foreach (var tag in MyItem1.Tags)
            {
                Console.WriteLine($"\thas tag.Name ({tag.Name})");
            }

            Assert.That(actualTags, Is.EqualTo(ExpectedTags));
        }

        private void InitDb1()
        {
            Console.WriteLine("init");
            using (var db = new ExpensesContext())
            {
                Console.WriteLine($"2Has ({db.Cards.Count()}) cards");
                db.Tags.Add(DanTag1);
                db.Tags.Add(RonTag1);
                db.Tags.Add(KerenTag1);
                db.SaveChanges();

                Console.WriteLine($"2Has ({db.Cards.Count()}) cards");
                Console.WriteLine($"2Has ({db.Statements.Count()}) statements");
                Console.WriteLine($"2Has ({db.Items.Count()}) items");
                Console.WriteLine($"2Has ({db.ItemTags.Count()}) item_tags");
                Console.WriteLine($"2Has ({db.Tags.Count()}) tags");
                db.Cards.Add(MyCard1);
                db.SaveChanges();
                Console.WriteLine($"MyCard.Id is {MyCard1.Id}");

                db.Statements.Add(MyStatement1);
                Console.WriteLine($"MyStatement1.CardId is {MyStatement1.CardId}");
                db.SaveChanges();

                db.Items.Add(MyItem1);
                // db.Items.Add(ItemWithRonAndDanTags);
                db.SaveChanges();

                // {
                //     var itemTags = new[]
                //     {
                //         new ItemTag
                //         {
                //             ItemId = ItemWithRonAndDanTags.Id,
                //                    TagId = DanTag.Id
                //         },
                //             new ItemTag
                //             {
                //                 ItemId = ItemWithRonAndDanTags.Id,
                //                 TagId = RonTag.Id
                //             }
                //     };
                //
                //     db.ItemTags.AddRange(itemTags);
                //     db.SaveChanges();
                //
                //     ItemWithRonAndDanTags.ItemTags.AddRange(itemTags);
                //     db.SaveChanges();
                // }
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
