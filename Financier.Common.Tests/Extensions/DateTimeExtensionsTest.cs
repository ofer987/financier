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
    }
}
