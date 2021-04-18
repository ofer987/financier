using System.Linq;
using NUnit.Framework;

using Financier.Common.Extensions;

namespace Financier.Common.Tests.Expenses.Models.StatementTests
{
    public class Delete : DatabaseFixture
    {
        [Test]
        [TestCase(FactoryData.Accounts.Dan.Cards.DanCard.CardNumber, 2)]
        [TestCase(FactoryData.Accounts.Ron.Cards.RonCard.CardNumber, 1)]
        public void Test_Expenses_Models_Statement_Delete_RemovesStatements(string cardNumber, int expectedRemoved)
        {
            using (var db = new Context())
            {
                var beforeCount = db.Statements.Count();

                // var statements = db.Cards
                //     .Include(card => card.Statements)
                //     .First(card => card.Number == cardNumber);
                    // .SelectMany(card => card.Statements);

                var statements = 
                    from stmts in db.Statements
                    join cards in db.Cards on stmts.CardId equals cards.Id
                    where cards.Number == cardNumber
                    select stmts;

                foreach (var statement in statements)
                {
                    statement.Delete();
                }
                    // .ForEach(stmt => Console.WriteLine(stmt.Id))
                    // .ForEach(stmt => stmt.Delete());

                var afterCount = db.Statements.Count(); 

                Assert.That(beforeCount - afterCount, Is.EqualTo(expectedRemoved));
            }
        }

        [Test]
        [TestCase(FactoryData.Accounts.Dan.Cards.DanCard.CardNumber, 5)]
        [TestCase(FactoryData.Accounts.Ron.Cards.RonCard.CardNumber, 2)]
        public void Test_Expenses_Models_Statement_Delete_RemovesItems(string cardNumber, int expectedRemoved)
        {
            using (var db = new Context())
            {
                var beforeCount = db.Items.Count();

                // var statements = db.Cards
                //     .Include(card => card.Statements)
                //     .First(card => card.Number == cardNumber);
                // .SelectMany(card => card.Statements);

                var statements = 
                    from stmts in db.Statements
                    join cards in db.Cards on stmts.CardId equals cards.Id
                    where cards.Number == cardNumber
                    select stmts;

                foreach (var statement in statements)
                {
                    statement.Delete();
                }
                // .ForEach(stmt => Console.WriteLine(stmt.Id))
                // .ForEach(stmt => stmt.Delete());

                var afterCount = db.Items.Count(); 

                Assert.That(beforeCount - afterCount, Is.EqualTo(expectedRemoved));
            }
        }
    }
}
