using NUnit.Framework;

using Financier.Cli;

namespace Financier.Cli.Tests.StatementTests
{
    public class GetAll
    {
        [SetUp]
        public void Setup()
        {
            
        }

        [Test]
        public void Test1()
        {
            var path = "/Users/ofer987/work/Financier/Financier.Cli.Tests/StatementsTests";
            var statements = new Statements(path);
            var files = statements.GetAll();

            // Assert.That(files.Count, Is.EqualTo(2));

            var expected = new [] { $"{path}/123345/20181103.csv", $"{path}/123345/20181203.csv"};
            Assert.That(files, Is.EqualTo(expected));
        }
    }
}
