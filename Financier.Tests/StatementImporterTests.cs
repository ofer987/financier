using System;
using System.Linq;
using NUnit.Framework;

using Financier.Common;

namespace Financier.Tests
{
    public class StatementImporterTests
    {
        [SetUp]
        public void Setup()
        {
            using (var db = new ExpensesContext())
            {
                // Delete all entities in db
            }
        }

        [Test]
        public void Test1()
        {
            var data = string.Empty;
            var buffer = data.ToCharArray().Cast<byte>().ToArray();
            var reader = new System.IO.MemoryStream(buffer);

            var statement = StatementImporter.Import(Guid.NewGuid(), new DateTime(2018, 12, 1), reader);

            Assert.IsTrue(statement.Items.Count == 1);
        }
    }
}
