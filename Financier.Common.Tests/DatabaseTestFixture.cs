using NUnit.Framework;

namespace Financier.Common.Tests
{
    [TestFixture]
    public abstract class DatabaseAbstractFixture
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

            InitDb();
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            Context.Clean();
        }

        protected abstract void InitDb();
    }
}
