using System.Linq;
using NUnit.Framework;

namespace Financier.Common.Tests.Expenses.Models.ItemTests
{
    public class Delete : Fixture
    {
        [Test]
        [TestCase(MyFactories.PorscheItemId, 1)]
        [TestCase(MyFactories.PorscheItemId, 1)]
        public void Test_Expenses_Models_Item_Delete_RemovesItem(string itemId, int expectedTagsRemoved)
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
        [TestCase(MyFactories.PorscheItemId, 2)]
        [TestCase(MyFactories.LamboItemId, 1)]
        public void Test_Expenses_Models_Item_Delete_RemovesItemTags(string itemId, int expectedTagsRemoved)
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
        [TestCase(MyFactories.PorscheItemId)]
        [TestCase(MyFactories.LamboItemId)]
        public void Test_Expenses_Models_Item_Delete_DoesNotRemoveTags(string itemId)
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
