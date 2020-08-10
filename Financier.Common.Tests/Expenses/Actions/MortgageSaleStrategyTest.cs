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

        [TestCase(2000.00, false, 2100)]
        [TestCase(2000.00, true, 2000)]
        [TestCase(1000.00, false, 1050)]
        [TestCase(1000.00, true, 1000)]
        public void Test_MortgageSaleStrategy_GetReturnedPrice(decimal remainingMortgageBalance, bool willMortgageBeTransferred, decimal expected)
        {
            Assert.That(
                new MortgageSaleStrategy(remainingMortgageBalance, willMortgageBeTransferred).GetReturnedPrice(),
                Is.EqualTo(expected)
            );
        }
    }
}

