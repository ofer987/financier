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

        [TestCase(2000.00, 500, true, 450)]
        [TestCase(2000.00, 500, false, 475)]
        [TestCase(1000.00, 0, true, 500)]
        [TestCase(1000.00, 0, false, 500)]
        public void Test_GetReturnedPrice(decimal requestedPrice, decimal remainingMortgageBalance, bool willTheMortgageBeTransferred, decimal expected)
        {
            Assert.That(
                new HomeSaleStrategy(requestedPrice, remainingMortgageBalance, willTheMortgageBeTransferred).GetReturnedPrice(),
                Is.EqualTo(expected)
            );
        }
    }
}

