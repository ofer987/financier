using System;
using System.Linq;
using NUnit.Framework;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

using Financier.Common;

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

        [Test]
        public void TestImport()
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

                var data = @"Item #,Card #,Transaction Date,Posting Date,Transaction Amount,Description
                    1,'5191230192755321',20181101,20181105,13.37,EMA TEI TORONTO ON
                    2,'5191230192755321',20181103,20181105,1.46,APL*ITUNES.COM/BILL 800-263-3394 ON";

                var buffer = data.ToCharArray().Select(ch => Convert.ToByte(ch)).ToArray();
                var reader = new System.IO.MemoryStream(buffer);
                // foreach (var by in buffer)
                // {
                //     Console.WriteLine(by);
                // }

                var statement = StatementImporter.Import(Guid.NewGuid(), new DateTime(2018, 12, 1), reader);

                Assert.IsTrue(statement.Items.Count() == 2);
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
    }
}
