using System.Linq;
using NUnit.Framework;

namespace Financier.Common.Tests.Expenses.Models.TagTests
{
    public class Delete : Fixture
    {
        [Test]
        [TestCase("fun")]
        [TestCase("fast")]
        public void Test_Expenses_Models_Tag_Delete_RemovesTag(string tagName)
        {
            using (var db = new Context())
            {
                var beforeCount = db.Tags.Count();

                db.Tags
                    .First(tag => tag.Name == tagName)
                    .Delete();

                var afterCount = db.Tags.Count(); 

                Assert.That(beforeCount - afterCount, Is.EqualTo(1));
            }
        }

        [Test]
        [TestCase("fun", 2)]
        [TestCase("fast", 2)]
        public void Test_Expenses_Models_Tag_Delete_RemovesItemTags(string tagName, int expectedRemoved)
        {
            using (var db = new Context())
            {
                var beforeCount = db.ItemTags.Count();

                db.Tags
                    .First(tag => tag.Name == tagName)
                    .Delete();

                var afterCount = db.ItemTags.Count(); 

                Assert.That(beforeCount - afterCount, Is.EqualTo(expectedRemoved));
            }
        }

        [Test]
        [TestCase("fun")]
        [TestCase("fast")]
        public void Test_Expenses_Models_Tag_Delete_DoesNotDeleteItem(string tagName)
        {
            using (var db = new Context())
            {
                var beforeCount = db.Items.Count();

                db.Tags
                    .First(tag => tag.Name == tagName)
                    .Delete();

                var afterCount = db.Items.Count(); 

                Assert.That(beforeCount, Is.EqualTo(afterCount));
            }
        }
    }
}
