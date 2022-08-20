using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using Microsoft.EntityFrameworkCore;

using Financier.Common.Expenses;
using Financier.Common.Expenses.Models;

namespace Financier.Common.Tests.Expenses
{
    public class CreditCardStatementFileTests
    {
        [AllowNull]
        public string AccountName { get; private set; }

        [OneTimeSetUp]
        public void InitAll()
        {
            Context.Environment = Environments.Test;
            AccountName = "Mr Bean";
        }

        [SetUp]
        public void Init()
        {
            Context.Clean();

            using (var db = new Context())
            {
                db.SaveChanges();
            }
        }

        [TearDown]
        public void Cleanup()
        {
            Context.Clean();
        }

        public static IEnumerable TestCases
        {
            get
            {
                yield return new TestCaseData(
                    new DateTime(2018, 11, 1, 0, 0, 0, DateTimeKind.Utc),
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
                                  PostedAt = new DateTime(2018, 11, 1, 0, 0, 0, DateTimeKind.Utc),
                                  Items = new List<Item>
                                  {
                                      new Item
                                      {
                                          ItemId = "1",
                                          Amount = 13.37M,
                                          Description = "EMA TEI TORONTO ON",
                                          TransactedAt = new DateTime(2018, 11, 1, 0, 0, 0, DateTimeKind.Utc),
                                          PostedAt = new DateTime(2018, 11, 5, 0, 0, 0, DateTimeKind.Utc),
                                      },
                                      new Item
                                      {
                                          ItemId = "2",
                                          Amount = 1.46M,
                                          Description = "APL*ITUNES.COM/BILL 800-263-3394 ON",
                                          TransactedAt = new DateTime(2018, 11, 3, 0, 0, 0, DateTimeKind.Utc),
                                          PostedAt = new DateTime(2018, 11, 5, 0, 0, 0, DateTimeKind.Utc),
                                      },
                                  }
                              }
                          }
                    });

                yield return new TestCaseData(
                    new DateTime(2018, 6, 1, 0, 0, 0, DateTimeKind.Utc),
                    @"Item #,Card #,Transaction Date,Posting Date,Transaction Amount,Description
                    1,'6171230192725321',20180601,20180602,13.37,EMA TEI TORONTO ON
                    2,'6171230192725321',20180602,20180605,1.46,APL*ITUNES.COM/BILL 800-263-3394 ON",
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
                                PostedAt = new DateTime(2018, 6, 1, 0, 0, 0, DateTimeKind.Utc),
                                Items = new List<Item>
                                {
                                    new Item
                                    {
                                        ItemId = "1",
                                        Amount = 13.37M,
                                        Description = "EMA TEI TORONTO ON",
                                        TransactedAt = new DateTime(2018, 6, 1, 0, 0, 0, DateTimeKind.Utc),
                                        PostedAt = new DateTime(2018, 6, 2, 0, 0, 0, DateTimeKind.Utc),
                                    },
                                    new Item
                                    {
                                        ItemId = "2",
                                        Amount = 1.46M,
                                        Description = "APL*ITUNES.COM/BILL 800-263-3394 ON",
                                        TransactedAt = new DateTime(2018, 6, 2, 0, 0, 0, DateTimeKind.Utc),
                                        PostedAt = new DateTime(2018, 6, 5, 0, 0, 0, DateTimeKind.Utc),
                                    },
                                }
                            }
                        }
                    }
                );
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

        [Test]
        [TestCaseSource(nameof(TestCases))]
        public void Test_Expenses_CreditCardStatementFileTests_Import_CardAlreadyExists(DateTime statementPostedAt, string statement, Card expectedCard)
        {
            var mrBean = Factories.CreateAccount("mr bean");
            using (var db = new Context())
            {
                db.Cards.Add(new Financier.Common.Expenses.Models.Card
                {
                    Id = Guid.NewGuid(),
                    Number = expectedCard.Number,
                    CardType = CardTypes.Credit,
                    AccountName = mrBean.Name
                });

                db.SaveChanges();
            }

            var buffer = statement.ToCharArray().Select(ch => Convert.ToByte(ch)).ToArray();
            var reader = new System.IO.MemoryStream(buffer);

            new CreditCardStatementFile(AccountName, reader, statementPostedAt).Import();

            using (var db = new Context())
            {
                var actual = db.Statements
                    .Include(stmt => stmt.Card)
                    .Include(stmt => stmt.Items)
                    .First();

                Assert.That(actual.Card.CardType, Is.EqualTo(expectedCard.CardType));
                Assert.That(actual.Card.Statements.Count, Is.EqualTo(expectedCard.Statements.Count));
                foreach (var s in actual.Card.Statements)
                {
                    var expectedStatement = expectedCard.Statements
                        .First(st => st.Month == s.Month && st.Year == s.Year);

                    Assert.That(s.PostedAt, Is.EqualTo(expectedStatement.PostedAt));

                    foreach (var item in s.Items)
                    {
                        var expectedItem = expectedStatement.Items
                            .First(i => i.ItemId == item.ItemId);

                        Assert.That(item.Description, Is.EqualTo(expectedItem.Description));
                        Assert.That(item.TransactedAt, Is.EqualTo(expectedItem.TransactedAt));
                        Assert.That(item.PostedAt, Is.EqualTo(expectedItem.PostedAt));
                        Assert.That(item.Amount, Is.EqualTo(expectedItem.Amount));
                    }
                }
            }
        }

        [Test]
        [TestCaseSource(nameof(TestCases))]
        public void Test_Expenses_CreditCardStatementFileTests_Import_CardDoesNotAlreadyExist(DateTime statementPostedAt, string statement, Card expectedCard)
        {
            var buffer = statement.ToCharArray().Select(ch => Convert.ToByte(ch)).ToArray();
            var reader = new System.IO.MemoryStream(buffer);

            Assert.Throws<DbUpdateException>(() => new CreditCardStatementFile(AccountName, reader, statementPostedAt).Import());
        }

        [Test]
        [TestCaseSource(nameof(CardNumbers))]
        public string Test_Expenses_CreditCardStatementFileTests_CleanCardNumber_Success(string unclean)
        {
            return new CreditCardStatementRecord().CleanNumber(unclean);
        }

        [Test]
        [TestCaseSource(nameof(FailureCardNumbers))]
        public void Test_Expenses_CreditCardStatementFileTests_CleanCardNumber_Fail(string unclean)
        {
            Assert.Throws<Exception>(() => new CreditCardStatementRecord().CleanNumber(unclean));
        }

        [Test]
        public void Test_Expenses_CreditCardStatementFileTests_SaveCard()
        {
            using (var db = new Context())
            {
                db.Database.EnsureCreated();

                var mrBean = Factories.NewAccount("mrbean");
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
                        PostedAt = new DateTime(2019, 2, 1, 0, 0, 0, DateTimeKind.Utc),
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
                        PostedAt = new DateTime(2019, 2, 2, 0, 0, 0, DateTimeKind.Utc),
                        Items = new List<Item>()
                    };
                    db.Statements.Add(newStatement);
                    db.SaveChanges();
                }
            }
        }

        [Test]
        public void Test_Expenses_CreditCardStatementFileTests_CreateItem()
        {
            try
            {
                using (var db = new Context())
                {
                    db.Database.EnsureCreated();

                    var owner = Factories.NewAccount("mr bean");
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
                        PostedAt = new DateTime(2019, 2, 1, 0, 0, 0, DateTimeKind.Utc),
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

                    var dbItem = db.Items.First(i => i.Amount == 10.00M);
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
        public void Test_Expenses_CreditCardStatementFileTests_SaveCardAndStatement()
        {
            Context.Clean();
            using (var db = new Context())
            {
                db.Database.EnsureCreated();

                var mrBean = Factories.NewAccount("mr bean");
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
                        PostedAt = new DateTime(2019, 2, 1, 0, 0, 0, DateTimeKind.Utc),
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
