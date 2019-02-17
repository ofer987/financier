using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

using Financier.Common;
using Financier.Common.Expenses;
using Financier.Common.Expenses.Models;

namespace Financier.Tests.Expenses.TagManagerTests
{
    public class FindOrCreateTags
    {
        public static IEnumerable TestCases
        {
            get
            {
                yield return new TestCaseData("hello, dan, ron").Returns(new [] 
                        {
                        new Tag
                        {
                        Id = Guid.NewGuid(),
                        Name = "hello"
                        },
                        new Tag
                        {
                        Id = Guid.NewGuid(),
                        Name = "dan"
                        },
                        new Tag
                        {
                        Id = Guid.NewGuid(),
                        Name = "ron"
                        }
                        });
                yield return new TestCaseData(", hello  , dan,ron, ron  , ").Returns(new [] 
                        {
                        new Tag
                        {
                        Id = Guid.NewGuid(),
                        Name = "hello"
                        },
                        new Tag
                        {
                        Id = Guid.NewGuid(),
                        Name = "dan"
                        },
                        new Tag
                        {
                        Id = Guid.NewGuid(),
                        Name = "ron"
                        }
                        });
                yield return new TestCaseData(string.Empty).Returns(new Tag[] { });
                yield return new TestCaseData("  ,   ,    ,").Returns(new Tag[] { });
                yield return new TestCaseData("Dan, dan, DAN, dAN").Returns(new [] 
                        {
                        new Tag
                        {
                        Id = Guid.NewGuid(),
                        Name = "dan"
                        }
                        });
            }
        }

        [OneTimeSetUp]
        public void InitAll()
        {
            Context.Environment = Environments.Test;
        }

        [SetUp]
        public void Init()
        {
            Context.Clean();
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            Context.Clean();
        }

        [Test]
        [TestCaseSource(nameof(TestCases))]
        public List<Tag> Test_Expenses_TagManager_FindOrCreateTags(string list)
        {
            return TagManager.FindOrCreateTags(list);
        }
    }
}
