using System;
using System.Reflection;
using System.IO;
using NUnit.Framework;

using Financier.Common.Expenses;

// TODO: move to Financier.Common.Tests
namespace Financier.Cli.Tests.StatementFileTests
{
    public class GetPostedAt
    {
        public string AccountName { get; private set; }

        [SetUp]
        public void Setup()
        {
            AccountName = "Mr Bean";
        }

        [Test]
        [TestCase("123345/20181203.csv", 2018, 12, 3)]
        [TestCase("123345/20181103.csv", 2018, 11, 3)]
        public void Test_Statements_GetPostedAt_PositiveTest(string relativePath, int year, int month, int day)
        {
            var expectedDate = new DateTime(year, month, day);
            var path = GetCsvPath(relativePath);

            Assert.That(new CreditCardStatementFile(AccountName, path).PostedAt, Is.EqualTo(expectedDate));
        }

        [Test]
        [TestCase("123345/file_does_not_exist31432.csv")]
        public void Test_Statements_SetPostedAt_NegativeTest(string relativePath)
        {
            var path = GetCsvPath(relativePath);

            Assert.Throws<FileNotFoundException>(() => new CreditCardStatementFile(AccountName, path));
        }

        private string GetCsvPath(string relativePath)
        {
            var path = Path.Join(Assembly.GetExecutingAssembly().Location, "..");
            path = Path.Join(path, "..");
            path = Path.Join(path, "..");
            path = Path.Join(path, "..");
            path = Path.Join(path, "StatementFileTests");

            return Path.Join(path, relativePath);
        }
    }
}
