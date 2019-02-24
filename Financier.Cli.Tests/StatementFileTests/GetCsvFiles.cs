using System.Reflection;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace Financier.Cli.Tests.StatementFileTests
{
    public class GetCsvFiles
    {
        [SetUp]
        public void Setup()
        {
            
        }

        [Test]
        public void Test_Statements_GetCsvFiles_ValidPath()
        {
            var path = GetCsvPath();
            var files = StatementFile.GetCsvFiles(path);

            var expected = new [] 
            { 
                new FileInfo(Path.Join(Path.Join(path, "123345"), "20181103.csv")),
                new FileInfo(Path.Join(Path.Join(path, "123345"), "20181203.csv"))
            };
            Assert.That(
                files.Select(file => file.Name),
                Is.EqualTo(expected.Select(file => file.Name))
            );
        }

        [Test]
        [TestCase("/")]
        public void Test_Statements_GetAll_UnauthorizedAccess(string path)
        {
            Assert.Throws<System.UnauthorizedAccessException>(() => StatementFile.GetCsvFiles(path));
        }

        [Test]
        [TestCase("/foobar")]
        [TestCase("   ")]
        public void Test_Statements_GetAll_DirectoryNotFound(string path)
        {
            Assert.Throws<System.IO.DirectoryNotFoundException>(() => StatementFile.GetCsvFiles(path));
        }

        [TestCase("")]
        public void Test_Statements_GetAll_EmptyPath(string path)
        {
            Assert.Throws<System.ArgumentException>(() => StatementFile.GetCsvFiles(path));
        }

        private string GetCsvPath()
        {
            var path = Path.Join(Assembly.GetExecutingAssembly().Location, "..");
            path = Path.Join(path, "..");
            path = Path.Join(path, "..");
            path = Path.Join(path, "..");

            return Path.Join(path, "StatementFileTests");
        }
    }
}
