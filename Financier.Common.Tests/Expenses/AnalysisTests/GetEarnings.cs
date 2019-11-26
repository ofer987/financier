using System;
using NUnit.Framework;

using Financier.Common.Expenses;

namespace Financier.Common.Tests.Expenses.AnalysisTests
{
    public class GetEarnings : Fixture
    {
        [Test]
        [TestCase(30, 2750.00)]
        [TestCase(14, 1283.33333333333)]
        [TestCase(1, 91.6666666666667)]
        public void Test_Expenses_Analysis_GetEarnings(int days, decimal expected)
        {
            Assert.That(
                new Analysis(DateTime.Now, DateTime.Now).GetEarnings(days),
                Is.EqualTo(expected)
            );
        }
    }
}
