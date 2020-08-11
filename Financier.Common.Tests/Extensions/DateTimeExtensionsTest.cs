using System;
using NUnit.Framework;

using Financier.Common.Extensions;

namespace Financier.Common.Tests.Extensions
{
    public class DateTimeExtensionsTest
    {
        [Test]
        public void Test_DateTimeExtensions_GetNext()
        {
            Assert.That(
                new DateTime(2019, 12, 31).GetNext(),
                Is.EqualTo(new DateTime(2020, 1, 31))
            );

            Assert.That(
                new DateTime(2020, 5, 31).GetNext(),
                Is.EqualTo(new DateTime(2020, 6, 30))
            );
        }

        [Test]
        public void Test_DateTimeExtensions_GetPrevious()
        {
            Assert.That(
                new DateTime(2020, 2, 29).GetPrevious(),
                Is.EqualTo(new DateTime(2020, 1, 29))
            );

            Assert.That(
                new DateTime(2020, 3, 31).GetPrevious(),
                Is.EqualTo(new DateTime(2020, 2, 29))
            );
        }

        [TestCase(2018, 1, 1, 2019, 1, 1, 12)]
        [TestCase(2018, 1, 1, 2020, 1, 1, 24)]
        [TestCase(2020, 2, 29, 2021, 2, 28, 12)]
        [TestCase(2020, 2, 29, 2023, 2, 28, 36)]
        [TestCase(2015, 3, 28, 2015, 3, 27, 0)]
        [TestCase(2015, 3, 28, 2015, 3, 28, 0)]
        [TestCase(2015, 3, 28, 2015, 3, 29, 0)]
        [TestCase(2015, 3, 28, 2015, 5, 27, 1)]
        [TestCase(2015, 3, 28, 2015, 5, 28, 2)]
        [TestCase(2015, 3, 28, 2015, 5, 29, 2)]
        public void Test_DateTimeExtensions_SubtractWholeMonths(int startYear, int startMonth, int startDay, int endYear, int endMonth, int endDay, int expected)
        {
            var start = new DateTime(startYear, startMonth, startDay);
            var end = new DateTime(endYear, endMonth, endDay);

            Assert.That(end.SubtractWholeMonths(start), Is.EqualTo(expected));
        }
    }
}
