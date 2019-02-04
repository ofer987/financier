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
        public void Init()
        {
            using (var db = new ExpensesContext())
            {
                db.Cards.RemoveRange(db.Cards);
                db.SaveChanges();
            }
        }

        [TearDown]
        public void Cleanup()
        {
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
            try
            {
                using (var db = new ExpensesContext())
                {
                    db.Database.EnsureCreated();
                    db.Cards.Add(new Financier.Common.Models.Expenses.Card { Id = Guid.NewGuid(), Number = "1234" });
                    Console.WriteLine("saved one card");
                    db.SaveChanges();
                }

                var buffer = statement.ToCharArray().Select(ch => Convert.ToByte(ch)).ToArray();
                var reader = new System.IO.MemoryStream(buffer);

                var actualStatement = StatementImporter.Import(Guid.NewGuid(), statementPostedAt, reader);

                Assert.That(actualStatement.Card, Is.EqualTo(expectedCard));
            }
            catch (Exception)
            {
                throw;
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
