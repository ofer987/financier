using System;
using System.Linq;
using NUnit.Framework;

using Financier.Common.Expenses.Actions;

namespace Financier.Common.Tests.Expenses.ActionTests
{
    public class HomePurchaseStrategyTest
    {
        [SetUp]
        public void Init()
        {
        }

        [TestCase(2000.00, 2019, 1, 1, 2000.00)]
        [TestCase(1000.00, 2019, 1, 2, 1000.00)]
        public void Test_GetReturnedPrice(decimal requestedPrice, int year, int month, int day, decimal expected)
        {
            Assert.That(
                new HomePurchaseStrategy(requestedPrice, new DateTime(year, month, day)).GetReturnedPrice(),
                Is.EqualTo(
                    new decimal[] {
                        expected,
                        1000.00M,
                        8500.00M,
                        800.00M
                    }.Sum()
                ));
        }
    }
}

