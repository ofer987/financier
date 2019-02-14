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
    public class TagManagerTests
    {
        [OneTimeSetUp]
        public void InitAll()
        {
            ExpensesContext.Environment = Environments.Test;
        }

        [SetUp]
        public void Init()
        {
            using (var db = new ExpensesContext())
            {
                db.Items.RemoveRange(db.Items);
                db.Statements.RemoveRange(db.Statements);
                db.Cards.RemoveRange(db.Cards);
                db.Tags.RemoveRange(db.Tags);
                db.ItemTags.RemoveRange(db.ItemTags);
                db.SaveChanges();
            }
        }

        [TearDown]
        public void Cleanup()
        {
            using (var db = new ExpensesContext())
            {
                db.Items.RemoveRange(db.Items);
                db.Statements.RemoveRange(db.Statements);
                db.Cards.RemoveRange(db.Cards);
                db.Tags.RemoveRange(db.Tags);
                db.ItemTags.RemoveRange(db.ItemTags);
                db.SaveChanges();
            }
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

        public static Card MyCard => new Card
        {
            Id = Guid.NewGuid(),
            Number = "1234",
            Statements = new List<Statement> { MyStatement }
        };

        public static Statement MyStatement => new Statement
        {
            Id = Guid.NewGuid(),
            PostedAt = new DateTime(2019, 1, 1)
        };

        public static Item ItemWithoutTags => new Item
        {
            Id = Guid.NewGuid(),
            Amount = 10.00M,
            Description = "Item-Without-Tags",
            TransactedAt = new DateTime(2019, 1, 2)
        };

        public static Item ItemWithRonAndDanTags => new Item
        {
            Id = Guid.NewGuid(),
            Amount = 20.00M,
            Description = "Item that has Dan and Ron tags",
            TransactedAt = new DateTime(2019, 1, 3),
            ItemTags = new List<ItemTag>
            {
                new ItemTag
                {
                    Tag = DanTag
                },
                new ItemTag
                {
                    Tag = RonTag
                }
            }
        };

        public static Tag DanTag => new Tag
        {
            Id = Guid.NewGuid(),
            Name = "dan"
        };

        public static Tag RonTag => new Tag
        {
            Id = Guid.NewGuid(),
            Name = "ron"
        };

        public static Tag KerenTag => new Tag
        {
            Id = Guid.NewGuid(),
            Name = "keren"
        };

        public static IEnumerable TestCases_Tags
        {
            get
            {
                yield return new TestCaseData(
                    ItemWithoutTags,
                    new List<Tag> { DanTag }
                ).Returns(new List<Tag> { DanTag });

                yield return new TestCaseData(
                    ItemWithoutTags,
                    new List<Tag> { DanTag, RonTag }
                ).Returns(new List<Tag> { DanTag, RonTag });

                yield return new TestCaseData(
                    ItemWithRonAndDanTags,
                    new List<Tag> { DanTag }
                ).Returns(new List<Tag> { DanTag, RonTag });

                yield return new TestCaseData(
                    ItemWithRonAndDanTags,
                    new List<Tag> { DanTag, RonTag }
                ).Returns(new List<Tag> { DanTag, RonTag });

                yield return new TestCaseData(
                    ItemWithRonAndDanTags,
                    new List<Tag> { DanTag, RonTag, KerenTag }
                ).Returns(new List<Tag> { DanTag, RonTag, KerenTag });
            }
        }

        [Test]
        [TestCaseSource(nameof(TestCases))]
        public List<Tag> Test_FindOrCreateTags(string list)
        {
            return TagManager.FindOrCreateTags(list);
        }

        [Test]
        [TestCaseSource(nameof(TestCases_Tags))]
        public List<Tag> Test_AddNewTags(Item item, List<Tag> tags)
        {
            if (new TagManager(item).TryAddTags(tags, out List<Tag> itemTags))
            {
                return null;
            }

            return itemTags;
        }
    }
}
