using System;
using NUnit.Framework;

using Financier.Common.Expenses;
using Financier.Common.Tests.Expenses.Models;

namespace Financier.Common.Tests.Expenses.AnalysisTests
{
    public class GetEarnings : Fixture
    {
        [Test]
        [TestCase(2019, 6, 33075.85)]
        [TestCase(2019, 7, 598102.45)]
        public void Test_Expenses_Analysis_GetEarnings(int year, int month, decimal expected)
        {
            // -3000 + 104.50 + 4.20 + 967.15 + 35000.00
            // -2000 + 98.25 + 4.20 + 600000
            var startAt = new DateTime(year, month, 1);
            var endAt = startAt.AddMonths(1).AddDays(-1);
            var actual = new Analysis(startAt, endAt).GetEarnings();

            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}
