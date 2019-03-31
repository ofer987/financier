using System;
using NUnit.Framework;

using Financier.Common.Expenses;

namespace Financier.Common.Tests.Expenses.AnalysisTests
{
    public class GetExpenses : Fixture
    {
        [Test]
        [TestCase(2019, 6, 32275.85)]
        [TestCase(2019, 7, 597302.45)]
        public void Test_Expenses_Analysis_GetExpenses(int year, int month, decimal expected)
        {
            // -3000 + 104.50 + 4.20 + 967.15 + 35000.00
            // -2000 + 98.25 + 4.20 + 600000
            var startAt = new DateTime(year, month, 1);
            var endAt = startAt.AddMonths(1).AddDays(-1);
            var actual = new Analysis(startAt, endAt).GetExpenses();

            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}
