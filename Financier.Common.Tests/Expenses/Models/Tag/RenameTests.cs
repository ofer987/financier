using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Microsoft.EntityFrameworkCore;

using TagModel = Financier.Common.Expenses.Models.Tag;

namespace Financier.Common.Tests.Expenses.Models.Tag
{
    public class RenameTests : InitializedDatabaseTests
    {
        [Test]
        [TestCase(FactoryData.Tags.Fun.Name, "super-fun")]
        public void Test_Expenses_Models_Tag_Rename_RenamesExistingTag(string existingName, string newName)
        {
            existingName = existingName.ToLower();
            newName = newName.ToLower();

            int beforeTagCount;
            int beforeItemTagCount;
            Guid beforeTagId;
            using (var db = new Context())
            {
                beforeTagCount = db.Tags.Count();
                beforeItemTagCount = db.ItemTags.Count();
            }

            if (TagModel.Get(newName) != null)
            {
                Assert.Fail($"A tag with the new name {newName} already exists!");
            }

            var tag = TagModel.GetOrCreate(existingName);
            beforeTagId = tag.Id;
            tag.Rename(newName);

            using (var db = new Context())
            {
                var afterTagCount = db.Tags.Count();
                var afterItemTagCount = db.ItemTags.Count();

                Assert.That(afterTagCount, Is.EqualTo(beforeTagCount), "A new tag has been created, while the old one has been deleted");
                Assert.That(afterItemTagCount, Is.EqualTo(beforeItemTagCount), "The same amount of relationships between items and tags should exist");
                Assert.That(db.Tags.First(t => t.Name == newName).Id, Is.Not.EqualTo(beforeTagId), "The tag is a new entity and thus should have a different Id");
            }
        }

        [Test]
        [TestCase(FactoryData.Tags.Fun.Name, FactoryData.Tags.Fast.Name)]
        [TestCase(FactoryData.Tags.Dog.Name, FactoryData.Tags.Fun.Name)]
        public void Test_Expenses_Models_Tag_Rename_RemovesExistingTag_NoDuplicates(string existingName, string newName)
        {
            existingName = existingName.ToLower();
            newName = newName.ToLower();

            int beforeTagCount;
            int beforeItemTagCount;
            var existingTag = TagModel.GetOrCreate(existingName);
            using (var db = new Context())
            {
                beforeTagCount = db.Tags.Count();
                beforeItemTagCount = db.ItemTags.Count();

            }
            var newTag = existingTag.Rename(newName);
            var newTagId = newTag.Id;

            using (var db = new Context())
            {
                var afterTagCount = db.Tags.Count();
                var afterItemTagCount = db.ItemTags.Count();

                Assert.IsNull(TagModel.Get(existingName));
                Assert.IsNotNull(TagModel.Get(newName));
                Assert.That(beforeTagCount - afterTagCount, Is.EqualTo(1), "A tag should have been removed");
                Assert.That(afterItemTagCount, Is.LessThanOrEqualTo(beforeItemTagCount), $"New ItemTags should not have been created if the existing item had previously been tagged with {newName}");
            }
        }

        private IEnumerable<string> GetItemTagNames()
        {
            using (var db = new Context())
            {
                var iTs = db.ItemTags
                    .Include(itemTags => itemTags.Tag)
                    .Include(itemTags => itemTags.Item);

                foreach (var itemTag in iTs)
                {
                    yield return $"{itemTag.Item.Description}\t{itemTag.Tag.Name}";
                }
            }
        }
    }
}
