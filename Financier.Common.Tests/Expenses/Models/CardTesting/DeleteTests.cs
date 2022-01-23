using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

using Financier.Common.Expenses.Models;

namespace Financier.Common.Tests.Expenses.Models.CardTests
{
    public class DeleteTests : InitializedDatabaseTests
    {
        [Test]
        [TestCase(FactoryData.Accounts.Dan.Cards.Savings.CardNumber, 1)]
        [TestCase(FactoryData.Accounts.Ron.Cards.RonCard.CardNumber, 1)]
        public void Test_Expenses_Models_CardTesting_DeleteTests_RemovesCard(string cardNumber, int expectedCardsRemoved)
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
        [TestCase(FactoryData.Accounts.Dan.Cards.Savings.CardNumber, 4)]
        [TestCase(FactoryData.Accounts.Ron.Cards.RonCard.CardNumber, 1)]
        public void Test_Expenses_Models_CardTesting_DeleteTests_RemovesStatements(string cardNumber, int expectedStatementsRemoved)
        {
            IEnumerable<Card> cards;
            int beforeCount;
            using (var db = new Context())
            {
                beforeCount = db.Statements.Count();

                cards =
                    (
                     from c in db.Cards
                     where c.Number == cardNumber
                     select c
                    ).AsEnumerable().ToArray();
            }

            foreach (var card in cards)
            {
                card.Delete();
            }

            using (var db = new Context())
            {
                var afterCount = db.Statements.Count();

                Assert.That(beforeCount - afterCount, Is.EqualTo(expectedStatementsRemoved));
            }
        }
    }
}
