using System;
using System.Linq;
using NUnit.Framework;

using Financier.Common.Expenses.Actions;
using Financier.Common.Models;

namespace Financier.Common.Tests.Expenses.ActionTests
{
    public class HomePurchaseStrategyTest
    {
        [SetUp]
        public void Init()
        {
        }

        [TestCase(2000.00, 2018, 1, 1, 2000.00)]
        [TestCase(1000.00, 2018, 1, 2, 1000.00)]
        [TestCase(1000.00, 2018, 12, 31, 1000.00)]
        [TestCase(1000.00, 2019, 1, 1, 1000.00)]
        public void Test_HomePurchaseStrategy_GetReturnedPrice(decimal requestedPrice, int year, int month, int day, decimal expected)
        {
            var requestedAt = new DateTime(year, month, day);
            Assert.That(
                new HomePurchaseStrategy(
                    requestedPrice,
                    requestedAt
                ).GetReturnedPrice(),
                Is.EqualTo(
                    new decimal[] {
                        expected,
                        Inflations.ConsumerPriceIndex
                            .GetValueAt(
                                1000.00M,
                                HomePurchaseStrategy.InflationStartsAt,
                                requestedAt
                            ),
                        Inflations.ConsumerPriceIndex
                            .GetValueAt(
                                8500.00M,
                                HomePurchaseStrategy.InflationStartsAt,
                                requestedAt
                            ),
                        Inflations.ConsumerPriceIndex
                            .GetValueAt(
                                800.00M,
                                HomePurchaseStrategy.InflationStartsAt,
                                requestedAt
                            )
                    }.Sum()
                ));
        }
    }
}

