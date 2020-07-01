using System;
using System.Collections.Generic;
using NUnit.Framework;

using Financier.Common.Models;
using Financier.Common.Expenses.Actions;

namespace Financier.Common.Tests.Expenses.ActionTests
{
    public class HomeSaleStrategyTest
    {
        [SetUp]
        public void Init()
        {
        }

        [TestCase(2000.00, 2019, 1, 1, 2000.00)]
        [TestCase(1000.00, 2019, 1, 2, 1000.00)]
        public void Test_GetReturnedPrice(decimal requestedPrice, int year, int month, int day, decimal expected)
        {
            var requested = new Money(
                requestedPrice,
                new DateTime(year, month, day)
            );

            Assert.That(
                new HomeSaleStrategy(requested).GetReturnedPrice(),
                Is.EquivalentTo(
                    new List<Money> { 
                        new Money(expected, new DateTime(year, month, day)),
                        new Money(-0.05M * expected, new DateTime(year, month, day))
                    }
                ));
        }
    }
}

