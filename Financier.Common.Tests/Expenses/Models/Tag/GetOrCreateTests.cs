using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;

using Financier.Common.Expenses.Models;

namespace Financier.Common.Tests.Expenses.TagManagerTests
{
    [TestFixtureSource(typeof(GetOrCreateTests))]
    public class GetOrCreateTests
    {
        [AllowNull]
        public Tag DanTag { get; set; }

        [AllowNull]
        public Tag RonTag { get; set; }

        [AllowNull]
        public Tag KerenTag { get; set; }

        [AllowNull]
        public List<Tag> ExistingTags { get; set; }

        [AllowNull]
        public Tag NewTag { get; set; }

        [OneTimeSetUp]
        public void InitAll()
        {
            Context.Environment = Environments.Test;
        }

        [SetUp]
        public void Init()
        {
            Context.Clean();
            InitVariables();
            InitDb();
        }

        [TearDown]
        public void Cleanup()
        {
            Context.Clean();
        }

        [Test]
        [TestCase("new-tag")]
        [TestCase("another-new-tag")]
        public void Test_Expenses_TagManager_UpdateTags_NewTags(string name)
        {
            var actual = Tag.GetOrCreate(name);
            Assert.That(actual.Name, Is.EqualTo(name));
        }

        [Test]
        [TestCase("new-tag", 1)]
        [TestCase("another-new-tag", 1)]
        [TestCase("keren", 0)]
        [TestCase("DaN", 0)]
        public void Test_Expenses_TagManager_UpdateTags_CreatesNewRecord(string name, int additionalTagCount)
        {
            Func<int> countTags = () =>
            {
                using (var db = new Context())
                {
                    return db.Tags.Count();
                }
            };

            var beforeCount = countTags();
            Tag.GetOrCreate(name);
            var afterCount = countTags();

            Assert.That(afterCount, Is.EqualTo(beforeCount + additionalTagCount));
        }

        [Test]
        [TestCase(null, typeof(ArgumentException))]
        [TestCase("", typeof(ArgumentException))]
        public void Test_Expenses_TagManager_UpdateTags_InvalidNewTags(string name, Type exceptionType)
        {
            Assert.Throws(exceptionType, () => Tag.GetOrCreate(name));
        }

        [Test]
        [TestCase("dan", "dan")]
        [TestCase("ron", "ron")]
        [TestCase("keren", "keren")]
        [TestCase("KEREN", "keren")]
        public void Test_Expenses_Models_Tag_ExistingTag(string name, string expected)
        {
            var actual = Tag.GetOrCreate(name);
            Assert.That(actual.Name, Is.EqualTo(expected));
        }

        private void InitVariables()
        {
            DanTag = Factories.DanTag();
            RonTag = Factories.RonTag();
            KerenTag = Factories.KerenTag();
        }

        private void InitDb()
        {
            using (var db = new Context())
            {
                db.Tags.Add(DanTag);
                db.Tags.Add(RonTag);
                db.Tags.Add(KerenTag);

                db.SaveChanges();
            }
        }
    }
}
