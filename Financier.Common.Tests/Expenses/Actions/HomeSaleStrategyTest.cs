using NUnit.Framework;

using Financier.Common.Expenses.Actions;

namespace Financier.Common.Tests.Expenses.ActionTests
{
    public class HomeSaleStrategyTest
    {
        [SetUp]
        public void Init()
        {
        }

        [TestCase(2000.00, 900)]
        [TestCase(1050.00, -2.5)]
        [TestCase(1000.00, -50)]
        [TestCase(0, -1000.00)]
        public void Test_HomeSaleStrategy_GetReturnedPrice(decimal requestedPrice, decimal expected)
        {
            Assert.That(
                new HomeSaleStrategy(requestedPrice, HomeSaleStrategy.InflationStartsAt).GetReturnedPrice(),
                Is.EqualTo(expected)
            );
        }
    }
}

