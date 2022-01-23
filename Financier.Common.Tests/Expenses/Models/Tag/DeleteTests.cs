using System.Linq;
using NUnit.Framework;

using Microsoft.EntityFrameworkCore;

namespace Financier.Common.Tests.Expenses.Models.Tag
{
    public class DeleteTests : InitializedDatabaseTests
    {
        [Test]
        [TestCase("fun")]
        [TestCase("fast")]
        public void Test_Expenses_Models_Tag_Delete_RemovesTag(string tagName)
        {
            int beforeTagCount;
            int beforeItemTagCount;
            using (var db = new Context())
            {
                beforeTagCount = db.Tags.AsEnumerable().Count();
                beforeItemTagCount = db.ItemTags
                    .Include(it => it.Tag)
                    .Where(it => it.Tag.Name == tagName)
                    .Count();

                db.Tags
                    .First(tag => tag.Name == tagName)
                    .Delete();
            }

            using (var db = new Context())
            {
                var afterCount = db.Tags.Count();
                var afterItemTagCount = db.ItemTags.Count();

                afterItemTagCount = db.ItemTags
                    .Include(it => it.Tag)
                    .Where(it => it.Tag.Name == tagName)
                    .Count();

                Assert.That(beforeTagCount - afterCount, Is.EqualTo(1));
                Assert.That(afterItemTagCount, Is.LessThanOrEqualTo(beforeItemTagCount));
            }
        }

        [Test]
        [TestCase("fun", 2)]
        [TestCase("fast", 2)]
        public void Test_Expenses_Models_Tag_Delete_RemovesItemTags(string tagName, int expectedRemoved)
        {
            int beforeCount;
            using (var db = new Context())
            {
                beforeCount = db.ItemTags.Count();

                db.Tags
                    .First(tag => tag.Name == tagName)
                    .Delete();
            }

            using (var db = new Context())
            {
                var afterCount = db.ItemTags.Count();

                Assert.That(beforeCount - afterCount, Is.EqualTo(expectedRemoved));
            }
        }

        [Test]
        [TestCase("fun")]
        [TestCase("fast")]
        public void Test_Expenses_Models_Tag_Delete_DoesNotDeleteItem(string tagName)
        {
            int beforeCount;
            using (var db = new Context())
            {
                beforeCount = db.Items.Count();

                db.Tags
                    .First(tag => tag.Name == tagName)
                    .Delete();
            }

            using (var db = new Context())
            {
                var afterCount = db.Items.Count();

                Assert.That(beforeCount, Is.EqualTo(afterCount));
            }
        }
    }
}
