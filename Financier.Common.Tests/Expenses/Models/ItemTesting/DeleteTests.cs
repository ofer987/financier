using System.Linq;
using NUnit.Framework;

namespace Financier.Common.Tests.Expenses.Models.ItemTesting
{
    public class DeleteTests : InitializedDatabaseTests
    {
        [Test]
        [TestCase(FactoryData.Accounts.Dan.Cards.DanCard.Statements.June.Items.Porsche.ItemId, 1)]
        [TestCase(FactoryData.Accounts.Dan.Cards.DanCard.Statements.June.Items.Porsche.ItemId, 1)]
        public void Test_Expenses_Models_ItemTesting_Delete_RemovesItem(string itemId, int expectedTagsRemoved)
        {
            using (var db = new Context())
            {
                var beforeCount = db.Items.Count();

                db.Items
                    .First(item => item.ItemId == itemId)
                    .Delete();

                var afterCount = db.Items.Count();

                Assert.That(afterCount, Is.EqualTo(beforeCount - expectedTagsRemoved));
            }
        }

        [Test]
        [TestCase(FactoryData.Accounts.Dan.Cards.DanCard.Statements.June.Items.Porsche.ItemId, 2)]
        [TestCase(FactoryData.Accounts.Ron.Cards.RonCard.Statements.Crazy.Items.Lambo.ItemId, 2)]
        [TestCase(FactoryData.Accounts.Dan.Cards.DanCard.Statements.June.Items.Ferrari.ItemId, 1)]
        public void Test_Expenses_Models_ItemTesting_Delete_RemovesItemTags(string itemId, int expectedTagsRemoved)
        {
            using (var db = new Context())
            {
                var beforeCount = db.ItemTags.Count();

                db.Items
                    .First(item => item.ItemId == itemId)
                    .Delete();

                var afterCount = db.ItemTags.Count();

                Assert.That(beforeCount - afterCount, Is.EqualTo(expectedTagsRemoved));
            }
        }

        [Test]
        [TestCase(FactoryData.Accounts.Dan.Cards.DanCard.Statements.June.Items.Porsche.ItemId)]
        [TestCase(FactoryData.Accounts.Ron.Cards.RonCard.Statements.Crazy.Items.Lambo.ItemId)]
        public void Test_Expenses_Models_ItemTesting_Delete_DoesNotRemoveTags(string itemId)
        {
            using (var db = new Context())
            {
                var beforeCount = db.Tags.Count();

                db.Items
                    .First(item => item.ItemId == itemId)
                    .Delete();

                var afterCount = db.Tags.Count();

                Assert.That(afterCount, Is.EqualTo(beforeCount));
            }
        }
    }
}
