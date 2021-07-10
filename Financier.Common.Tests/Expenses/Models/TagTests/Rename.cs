using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Microsoft.EntityFrameworkCore;

using Financier.Common.Expenses.Models;

namespace Financier.Common.Tests.Expenses.Models.TagTests
{
    public class Rename : DatabaseFixture
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

                var tag = db.Tags.First(t => t.Name == existingName);
                beforeTagId = tag.Id;

                tag.Rename(newName);
            }

            using (var db = new Context())
            {
                var afterTagCount = db.Tags.Count();
                var afterItemTagCount = db.ItemTags.Count();

                // A new tag has been created, but the existing one was deleted
                Assert.That(afterTagCount, Is.EqualTo(beforeTagCount));
                Assert.That(afterItemTagCount, Is.LessThanOrEqualTo(beforeItemTagCount));
                Assert.That(db.Tags.First(t => t.Name == newName).Id, Is.Not.EqualTo(beforeTagId)); // The new tag is a new entity!
            }
        }

        [Test]
        [TestCase(FactoryData.Tags.Dog.Name, FactoryData.Tags.Fun.Name)]
        public void Test_Expenses_Models_Tag_Rename_RemovesExistingTag(string existingName, string newName)
        {
            existingName = existingName.ToLower();
            newName = newName.ToLower();

            int beforeTagCount;
            int beforeItemTagCount;
            Guid existingTagId;
            Tag existingTag;
            using (var db = new Context())
            {
                beforeTagCount = db.Tags.Count();
                beforeItemTagCount = db.ItemTags.Count();

                existingTag = db.Tags
                    .Include(tag => tag.ItemTags)
                        .ThenInclude(it => it.Item)
                    .AsEnumerable()
                    .First(t => t.Name == existingName);
            }

                // var tag = db.Tags
                //     .Include(tag => tag.ItemTags)
                //         .ThenInclude(it => it.Item)
                //     .First(t => t.Name == existingName);
                // beforeTagId = tag.Id;

                // foreach (var name in GetItemTagNames())
                // {
                //     Console.WriteLine(name);
                // }

            existingTagId = existingTag.Id;
            using (var db = new Context())
            {
                Console.WriteLine($"Before Rename: {db.Tags.Count()}");
                var count = db.ItemTags
                    .Where(it => it.TagId == existingTagId)
                    .AsEnumerable();

                Console.WriteLine($"There are {count} remaining");
            }
            existingTag.Rename(newName);
            using (var db = new Context())
            {
                Console.WriteLine($"After Rename: {db.Tags.Count()}");
                var count = db.ItemTags
                    .Where(it => it.TagId == existingTagId)
                    .AsEnumerable();

                Console.WriteLine($"There are {count} remaining");
            }
            // using (var db = new Context())
            // {
            //     Console.WriteLine($"Id of existing Tag: {db.Tags.First(tag => tag.Id == existingTag.Id).Id}");
            //     Console.WriteLine($"After Rename: {db.Tags.Count()}");
            //
            //     var existingItemTags = db.ItemTags.Where(it => it.TagId == existingTagId).AsEnumerable();
            //     if (existingItemTags.Count() > 0)
            //     {
            //         Console.WriteLine($"There are still {existingItemTags.Count()}");
            //     }
            //     // db.Tags.Remove(existingTag);
            //     db.SaveChanges();
            // }
            // existingTag.Delete();
            // db.SaveChanges();

            // db.Tags.Remove(tag);
            // db.SaveChanges();
            // }

            Console.WriteLine();

            using (var db = new Context())
            {
                foreach (var name in GetItemTagNames())
                {
                    Console.WriteLine(name);
                }

                var afterTagCount = db.Tags.Count();
                var afterItemTagCount = db.ItemTags.Count();

                Assert.That(db.Tags.First(t => t.Name == newName).Id, Is.Not.EqualTo(existingTagId));
                Assert.That(beforeTagCount - afterTagCount, Is.EqualTo(1));
                Assert.That(afterItemTagCount, Is.LessThan(beforeItemTagCount));
            }
        }

        [Test]
        [TestCase(FactoryData.Tags.Fun.Name, FactoryData.Tags.Fast.Name)]
        public void Test_Expenses_Models_Tag_Rename_RemovesExistingTag_NoDuplicates(string existingName, string newName)
        {
            existingName = existingName.ToLower();
            newName = newName.ToLower();

            int beforeTagCount;
            int beforeItemTagCount;
            // Guid existingTagId;
            // Guid newTagId;
            var existingTag = Tag.GetOrCreate(existingName);
            using (var db = new Context())
            {
                beforeTagCount = db.Tags.Count();
                beforeItemTagCount = db.ItemTags.Count();

                // existingTag = db.Tags.First(t => t.Name == existingName);
                // existingTagId = existingTag.Id;
                // newTagId = db.Tags.First(t => t.Name == newName).Id;

            }
            var newTag = existingTag.Rename(newName);
            var newTagId = newTag.Id;

            using (var db = new Context())
            {
                var afterTagCount = db.Tags.Count();
                var afterItemTagCount = db.ItemTags.Count();

                // A new tag has not been created
                Assert.That(beforeTagCount - afterTagCount, Is.EqualTo(1));

                // New ItemTags have been created
                Assert.That(afterItemTagCount, Is.Not.EqualTo(beforeItemTagCount));
                Console.WriteLine(afterItemTagCount);
                Assert.That(beforeItemTagCount, Is.GreaterThan(afterItemTagCount));

                // The renamed tag is different
                Assert.That(db.Tags.First(t => t.Name == newName).Id, Is.Not.EqualTo(existingTag.Id));
                Assert.That(db.Tags.First(t => t.Name == newName).Id, Is.EqualTo(newTagId));
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
                Assert.That(db.Tags.FirstOrDefault(t => t.Name == "super-fun"), Is.Not.EqualTo(null));
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
