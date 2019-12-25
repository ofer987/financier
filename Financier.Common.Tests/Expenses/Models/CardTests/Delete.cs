using System.Linq;
using NUnit.Framework;

namespace Financier.Common.Tests.Expenses.Models.CardTests
{
    public class Delete : DatabaseFixture
    {
        [Test]
        [TestCase(ModelFactories.DanCard.CardNumber, 1)]
        [TestCase(ModelFactories.RonCard.CardNumber, 1)]
        public void Test_Expenses_Models_Card_Delete_RemovesCard(string cardNumber, int expectedCardsRemoved)
        {
            using (var db = new Context())
            {
                var beforeCount = db.Cards.Count();

                db.Cards
                    .First(card => card.Number == cardNumber)
                    .Delete();

                var afterCount = db.Cards.Count(); 

                Assert.That(beforeCount - afterCount, Is.EqualTo(expectedCardsRemoved));
            }
        }

        [Test]
        [TestCase(ModelFactories.DanCard.CardNumber, 2)]
        [TestCase(ModelFactories.RonCard.CardNumber, 1)]
        public void Test_Expenses_Models_Card_Delete_RemovesStatements(string cardNumber, int expectedStatementsRemoved)
        {
            using (var db = new Context())
            {
                var beforeCount = db.Statements.Count();

                var cards = 
                    from c in db.Cards
                    where c.Number == cardNumber
                    select c;

                foreach (var card in cards)
                {
                    card.Delete();
                }

                var afterCount = db.Statements.Count(); 

                Assert.That(beforeCount - afterCount, Is.EqualTo(expectedStatementsRemoved));
            }
        }
    }
}
