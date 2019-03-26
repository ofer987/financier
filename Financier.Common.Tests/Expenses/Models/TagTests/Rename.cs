using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Microsoft.EntityFrameworkCore;

namespace Financier.Common.Tests.Expenses.Models.TagTests
{
    public class Rename : Fixture
    {
        [Test]
        [TestCase("fun", "super-fun")]
        public void Test_Expenses_Models_Tag_Rename_RenamesExistingTag(string existingName, string newName)
        {
            int beforeTagCount;
            int beforeItemTagCount;
            Guid beforeTagId;
            using (var db = new Context())
            {
                beforeTagCount = db.Tags.Count();
                beforeItemTagCount = db.ItemTags.Count();

                var tag = db.Tags.First(t => t.Name == existingName);
                beforeTagId = tag.Id;

                tag.Rename(newName);
            }

            using (var db = new Context())
            {
                var afterTagCount = db.Tags.Count(); 
                var afterItemTagCount = db.ItemTags.Count();

                Assert.That(afterTagCount, Is.EqualTo(beforeTagCount));
                Assert.That(afterItemTagCount, Is.EqualTo(beforeItemTagCount));
                Assert.That(db.Tags.First(t => t.Name == newName).Id, Is.EqualTo(beforeTagId));
            }
        }

        [Test]
        [TestCase("like-a-dog", "fun")]
        public void Test_Expenses_Models_Tag_Rename_RemovesExistingTag(string existingName, string newName)
        {
            int beforeTagCount;
            int beforeItemTagCount;
            Guid beforeTagId;
            using (var db = new Context())
            {
                beforeTagCount = db.Tags.Count();
                beforeItemTagCount = db.ItemTags.Count();

                var tag = db.Tags.First(t => t.Name == existingName);
                beforeTagId = tag.Id;

                foreach (var name in GetItemTagNames())
                {
                    Console.WriteLine(name);
                }

                tag.Rename(newName);
            }

            Console.WriteLine();

            using (var db = new Context())
            {
                foreach (var name in GetItemTagNames())
                {
                    Console.WriteLine(name);
                }
                
                var afterTagCount = db.Tags.Count(); 
                var afterItemTagCount = db.ItemTags.Count();

                Assert.That(db.Tags.First(t => t.Name == newName).Id, Is.Not.EqualTo(beforeTagId));
                Assert.That(beforeTagCount - afterTagCount, Is.EqualTo(1));
                Assert.That(afterItemTagCount, Is.EqualTo(beforeItemTagCount));
            }
        }

        [Test]
        [TestCase("fun", "fast")]
        public void Test_Expenses_Models_Tag_Rename_RemovesExistingTag_NoDuplicates(string existingName, string newName)
        {
            int beforeTagCount;
            int beforeItemTagCount;
            Guid beforeTagId;
            using (var db = new Context())
            {
                beforeTagCount = db.Tags.Count();
                beforeItemTagCount = db.ItemTags.Count();

                var tag = db.Tags.First(t => t.Name == existingName);
                beforeTagId = tag.Id;

                tag.Rename(newName);
            }

            using (var db = new Context())
            {
                var afterTagCount = db.Tags.Count(); 
                var afterItemTagCount = db.ItemTags.Count();

                Assert.That(beforeTagCount - afterTagCount, Is.EqualTo(1));
                Assert.That(afterItemTagCount, Is.Not.EqualTo(beforeItemTagCount));
                Assert.That(db.Tags.First(t => t.Name == newName).Id, Is.Not.EqualTo(beforeTagId));
            }
        }

        [Test]
        public void RenameExistingTag()
        {
            using (var db = new Context())
            {
                db.Tags
                    .First(t => t.Name == "fun")
                    .Name = "super-fun";
                db.SaveChanges();
            }

            using (var db = new Context())
            {
                Assert.That(db.Tags.DefaultIfEmpty(null).FirstOrDefault(t => t.Name == "super-fun"), Is.Not.EqualTo(null));
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
