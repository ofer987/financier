using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Microsoft.EntityFrameworkCore;

using Financier.Common.Expenses;
using Financier.Common.Expenses.Models;

namespace Financier.Common.Tests.Expenses.CreditCardStatementFileTests
{
    public class Base
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

            using (var db = new Context())
            {
                db.Accounts.Add(MrBean);
                db.SaveChanges();
            }
        }

        [TearDown]
        public void Cleanup()
        {
            Context.Clean();
        }

        public static Account MrBean = Factories.CreateAccount("mr bean");

        public static IEnumerable TestCases
        {
            get
            {
                yield return new TestCaseData(
                    new DateTime(2018, 11, 1),
                    @"Item #,Card #,Transaction Date,Posting Date,Transaction Amount,Description
1,'5191230192755321',20181101,20181105,13.37,EMA TEI TORONTO ON
        2,'5191230192755321',20181103,20181105,1.46,APL*ITUNES.COM/BILL 800-263-3394 ON",
                    new Card
                    {
                        Id = Guid.NewGuid(),
                        Number = "5191230192755321",
                        CardType = CardTypes.Credit,
                        Statements = new List<Statement>
                          {
                              new Statement
                              {
                                  Id = Guid.NewGuid(),
                                  PostedAt = new DateTime(2018, 11, 1),
                                  Items = new List<Item>
                                  {
                                      new Item
                                      {
                                          ItemId = Guid.NewGuid().ToString(),
                                          Amount = 13.37M,
                                          Description = "EMA TEI TORONTO ON",
                                          TransactedAt = new DateTime(2018, 11, 1),
                                          PostedAt = new DateTime(2018, 11, 5),
                                      },
                                      new Item
                                      {
                                          ItemId = Guid.NewGuid().ToString(),
                                          Amount = 1.46M,
                                          Description = "APL*ITUNES.COM/BILL 800-263-3394 ON",
                                          TransactedAt = new DateTime(2018, 11, 3),
                                          PostedAt = new DateTime(2018, 11, 5),
                                      },
                                  }
                              }
                          }
                    });

                yield return new TestCaseData(
                        new DateTime(2019, 1, 1),
                        @"Item #,Card #,Transaction Date,Posting Date,Transaction Amount,Description
                        1,'6171230192725321',20180601,20180601,13.37,EMA TEI TORONTO ON
                        2,'6171230192725321',20180602,20180602,1.46,APL*ITUNES.COM/BILL 800-263-3394 ON",
                        new Card
                        {
                            Id = Guid.NewGuid(),
                            Number = "6171230192725321",
                            CardType = CardTypes.Credit,
                            Statements = new List<Statement>
                        {
                        new Statement
                        {
                        Id = Guid.NewGuid(),
                        PostedAt = new DateTime(2019, 1, 1),
                        Items = new List<Item>
                        {
                        new Item
                        {
                        ItemId = Guid.NewGuid().ToString(),
                        Amount = 13.37M,
                        Description = "EMA TEI TORONTO ON",
                        TransactedAt = new DateTime(2018, 6, 1),
                        PostedAt = new DateTime(2018, 6, 2),
                        },
                        new Item
                        {
                            ItemId = Guid.NewGuid().ToString(),
                            Amount = 1.46M,
                            Description = "APL*ITUNES.COM/BILL 800-263-3394 ON",
                            TransactedAt = new DateTime(2018, 11, 3),
                            PostedAt = new DateTime(2018, 11, 5),
                        },
                        }
                        }
                        }
                        });
            }
        }

        public static IEnumerable CardNumbers
        {
            get
            {
                yield return new TestCaseData("'123456'").Returns("123456");
                yield return new TestCaseData("123456").Returns("123456");
                yield return new TestCaseData("'1234567'").Returns("1234567");
                yield return new TestCaseData("1234567").Returns("1234567");
                yield return new TestCaseData("  '   1234567 \t' ").Returns("1234567");
                yield return new TestCaseData("  '   123dan1234 \t' ").Returns("123dan1234");
            }
        }

        public static IEnumerable FailureCardNumbers
        {
            get
            {
                yield return new TestCaseData("");
                yield return new TestCaseData(string.Empty);
                yield return new TestCaseData(" '    '    ");
                yield return new TestCaseData(" '  '''  '    ");
            }
        }

        // TODO: this test is broken.
        // TODO: fix it (maybe Models.Statement too)
        [Test]
        [TestCaseSource(nameof(TestCases))]
        [Ignore("The test cases are not configured properly")]
        public void Test_Expenses_CreditCardStatementFile_Import_CardDoesNotAlreadyExist(DateTime statementPostedAt, string statement, Card expectedCard)
        {
            try
            {
                Context.Clean();
                var mrBean = Factories.CreateAccount("mr bean");
                using (var db = new Context())
                {
                    db.Accounts.Add(mrBean);

                    db.Cards.Add(new Financier.Common.Expenses.Models.Card { Id = Guid.NewGuid(), Number = expectedCard.Number, Owner = mrBean });
                    db.SaveChanges();
                }

                var buffer = statement.ToCharArray().Select(ch => Convert.ToByte(ch)).ToArray();
                var reader = new System.IO.MemoryStream(buffer);

                new CreditCardStatementFile(reader, statementPostedAt).Import();

                using (var db = new Context())
                {
                    var actual = db.Statements
                        .Include(stmt => stmt.Card)
                        .Include(stmt => stmt.Items)
                        .FirstOrDefault();

                    Assert.That(actual.Card, Is.EqualTo(expectedCard));
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        [Test]
        [TestCaseSource(nameof(TestCases))]
        [Ignore("The test cases are not configured properly")]
        public void Test_Expenses_CreditCardStatementFile_Import_CardAlreadyExists(DateTime statementPostedAt, string statement, Card expectedCard)
        {
            try
            {
                using (var db = new Context())
                {
                    db.Database.EnsureCreated();
                    db.Items.RemoveRange(db.Items);
                    db.Statements.RemoveRange(db.Statements);
                    db.Cards.RemoveRange(db.Cards);
                    db.Accounts.RemoveRange(db.Accounts);
                    db.SaveChanges();

                    db.Database.EnsureCreated();

                    var mrBean = Factories.CreateAccount("mr bean");
                    db.Accounts.Add(mrBean);

                    db.Cards.Add(new Financier.Common.Expenses.Models.Card
                    {
                        Owner = mrBean,
                        Id = Guid.NewGuid(),
                        CardType = CardTypes.Credit,
                        Number = expectedCard.Number
                    });
                    db.SaveChanges();
                }

                var buffer = statement.ToCharArray().Select(ch => Convert.ToByte(ch)).ToArray();
                var reader = new System.IO.MemoryStream(buffer);

                new CreditCardStatementFile(reader, statementPostedAt).Import();

                using (var db = new Context())
                {
                    var actual = db.Statements
                        .Include(stmt => stmt.Card)
                        .Include(stmt => stmt.Items)
                        .FirstOrDefault();

                    Assert.That(actual.Card, Is.EqualTo(expectedCard));
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        [Test]
        [TestCaseSource(nameof(CardNumbers))]
        public string Test_Expenses_CreditCardStatementRecord_CleanCardNumber_Success(string unclean)
        {
            return new CreditCardStatementRecord().CleanNumber(unclean);
        }

        [Test]
        [TestCaseSource(nameof(FailureCardNumbers))]
        public void Test_Expenses_CreditCardStatementRecord_CleanCardNumber_Fail(string unclean)
        {
            Assert.Throws<Exception>(() => new CreditCardStatementRecord().CleanNumber(unclean));
        }

        [Test]
        public void Test_Expenses_CreditCardStatementFile_SaveCard()
        {
            Context.Clean();
            using (var db = new Context())
            {
                db.Database.EnsureCreated();

                var mrBean = Factories.CreateAccount("mrbean");
                db.Accounts.Add(mrBean);

                var card = new Financier.Common.Expenses.Models.Card
                {
                    Owner = mrBean,
                    Id = Guid.NewGuid(),
                    Number = "1234",
                    Statements = new List<Statement>()
                };
                db.Cards.Add(card);
                db.SaveChanges();

                {
                    var newStatement = new Statement
                    {
                        Card = card,
                        Id = Guid.NewGuid(),
                        PostedAt = new DateTime(2019, 2, 1),
                        Items = new List<Item>()
                    };
                    db.Statements.Add(newStatement);
                    db.SaveChanges();
                }

                {
                    var newStatement = new Statement
                    {
                        Card = card,
                        Id = Guid.NewGuid(),
                        PostedAt = new DateTime(2019, 2, 2),
                        Items = new List<Item>()
                    };
                    db.Statements.Add(newStatement);
                    db.SaveChanges();
                }
            }
        }

        [Test]
        public void Test_Expenses_CreditCardStatementFile_CreateItem()
        {
            try
            {
                Context.Clean();

                using (var db = new Context())
                {
                    db.Database.EnsureCreated();

                    var owner = Factories.CreateAccount("mr bean");
                    db.Accounts.Add(owner);
                    db.SaveChanges();

                    var card = new Financier.Common.Expenses.Models.Card
                    {
                        Owner = owner,
                        Id = Guid.NewGuid(),
                        Number = "1234",
                        Statements = new List<Statement>()
                    };
                    db.Cards.Add(card);
                    db.SaveChanges();

                    var newStatement = new Statement
                    {
                        Card = card,
                        Id = Guid.NewGuid(),
                        PostedAt = new DateTime(2019, 2, 1),
                        Items = new List<Item>()
                    };
                    db.Statements.Add(newStatement);
                    db.SaveChanges();

                    var record = new CreditCardStatementRecord
                    {
                        Amount = "10.00",
                        Number = "1234",
                        Description = "Some new item",
                        ItemId = "123",
                        PostedAt = "20181103",
                        TransactedAt = "20181104"
                    };
                    record.CreateItem(newStatement.Id);

                    var dbItem = db.Items.FirstOrDefault(i => i.Amount == 10.00M);
                    Assert.That(dbItem.Statement, Is.EqualTo(newStatement));
                }
            }
            catch (Exception exception)
            {
                Assert.Fail(exception.StackTrace);
                Assert.Fail(exception.Message);
            }
        }

        [Test]
        public void Test_Expenses_CreditCardStatementFile_SaveCardAndStatement()
        {
            Context.Clean();
            using (var db = new Context())
            {
                db.Database.EnsureCreated();

                var mrBean = Factories.CreateAccount("mr bean");
                db.Accounts.Add(mrBean);

                var card = new Financier.Common.Expenses.Models.Card
                {
                    Owner = mrBean,
                    Id = Guid.NewGuid(),
                    Number = "1234",
                    Statements = new List<Statement>()
                };
                db.Cards.Add(card);
                db.SaveChanges();

                {
                    var newStatement = new Statement
                    {
                        Card = card,
                        Id = Guid.NewGuid(),
                        PostedAt = new DateTime(2019, 2, 1),
                        Items = new List<Item>()
                    };

                    card.Statements.Add(newStatement);
                    db.Statements.Add(newStatement);
                    Assert.That(db.Statements.Count, Is.EqualTo(0));
                    db.SaveChanges();
                }

                Assert.That(db.Cards.Count, Is.EqualTo(1));
                Assert.That(db.Statements.Count, Is.EqualTo(1));
            }
        }
    }
}
