using NUnit.Framework;

using Financier.Common.Expenses.Actions;

namespace Financier.Common.Tests.Expenses.ActionTests
{
    public class MortgageSaleStrategyTest
    {
        [SetUp]
        public void Init()
        {
        }

        [TestCase(2000.00, false, 100)]
        [TestCase(2000.00, true, 0)]
        [TestCase(1000.00, false, 50)]
        [TestCase(1000.00, true, 0)]
        public void Test_MortgageSaleStrategy_GetReturnedPrice(decimal remainingMortgageBalance, bool willMortgageBeTransferred, decimal expected)
        {
            Assert.That(
                new MortgageSaleStrategy(remainingMortgageBalance, willMortgageBeTransferred).GetReturnedPrice(),
                Is.EqualTo(expected)
            );
        }
    }
}

