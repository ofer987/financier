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
    public class StatementImporterTests
    {
        [SetUp]
        public void Setup()
        {
            using (var db = new ExpensesContext())
            {
                Console.WriteLine(db.Database.EnsureCreated());
                Console.WriteLine(db.Database.CanConnect());

                db.Cards.RemoveRange(db.Cards);
                db.SaveChanges();

                db.Cards.ToList();
                Console.WriteLine(db.Cards.Count());
                Console.WriteLine(db.Statements.Count());
                Console.WriteLine(db.Items.Count());
                // Console.WriteLine($"Has {db.Cards.First().Statements.Count} statements");
                // Delete all entities in db
                // if (db.Cards != null)
                // {
                //     db.RemoveRange(db.Cards);
                // }
            }
        }

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
                                          Amount = 13.37M,
                                          Description = "EMA TEI TORONTO ON",
                                          TransactedAt = new DateTime(2018, 11, 1),
                                          PostedAt = new DateTime(2018, 11, 5),
                                      },
                                      new Item
                                      {
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

        [Test]
        [TestCaseSource(nameof(TestCases))]
        public void TestImport(DateTime statementPostedAt, string statement, Card expectedCard)
        {
            // var connection = new SqliteConnection("DataSource=:memory:");
            // connection.Open();

            try
            {
                // var options = new DbContextOptionsBuilder<ExpensesContext>()
                //     .UseSqlite(connection)
                //     .Options;

                using (var db = new ExpensesContext())
                {
                    db.Database.EnsureCreated();
                    db.Cards.Add(new Financier.Common.Models.Expenses.Card { Id = Guid.NewGuid(), Number = "1234" });
                    Console.WriteLine("saved one card");
                    db.SaveChanges();
                }

                var buffer = statement.ToCharArray().Select(ch => Convert.ToByte(ch)).ToArray();
                var reader = new System.IO.MemoryStream(buffer);
                // foreach (var by in buffer)
                // {
                //     Console.WriteLine(by);
                // }

                var result = StatementImporter.Import(Guid.NewGuid(), statementPostedAt, reader);

                Assert.AreEqual(2, result.Items.Count());
                Assert.AreEqual(expectedCard.Number, result.Card.Number);
                Assert.AreEqual(expectedCard.Statements.Count, result.Card.Statements.Count);

                for (var i = 0; i < result.Card.Statements.Count; i += 1)
                {
                    var resultStatement = result.Card.Statements[i];
                    var expectedStatement = expectedCard.Statements[i];

                    Assert.AreEqual(expectedStatement.PostedAt, resultStatement.PostedAt);
                    Assert.AreEqual(expectedStatement.Items.Count, resultStatement.Items.Count);

                    for (var j = 0; j < resultStatement.Items.Count; j += 1)
                    {
                        var resultItem = resultStatement.Items[j];
                        var expectedItem = expectedStatement.Items[j];

                        Assert.AreEqual(expectedItem.Amount, resultItem.Amount);
                        Assert.AreEqual(expectedItem.Description, resultItem.Description);
                        Assert.AreEqual(expectedItem.PostedAt, resultItem.PostedAt);
                        Assert.AreEqual(expectedItem.TransactedAt, resultItem.TransactedAt);

                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                // connection.Close();
            }
        }

        [Test]
        [TestCaseSource(nameof(CardNumbers))]
        public string TestCleanCardNumber_Success(string unclean)
        {
            return StatementImporter.CleanCardNumber(unclean);
        }

        [Test]
        [TestCaseSource(nameof(FailureCardNumbers))]
        public void TestCleanCardNumber_Fail(string unclean)
        {
            Assert.Throws<Exception>(() => StatementImporter.CleanCardNumber(unclean));
        }
    }
}
