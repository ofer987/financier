using NUnit.Framework;

namespace Financier.Common.Tests
{
    [TestFixture]
    public abstract class Tests
    {
        [OneTimeSetUp]
        public void InitAll()
        {
            Context.Environment = Environments.Test;
        }

        [SetUp]
        public void Init()
        {
            Context.Clean();

            InitStorage();
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            // System.Console.ReadKey();
            Context.Clean();
        }

        protected abstract void InitStorage();
    }
}
