using System;
using NUnit.Framework;

using Financier.Common.Expenses;

namespace Financier.Common.Tests.Expenses.AnalysisTests
{
    public class GetEarnings : Fixture
    {
        [Test]
        [TestCase(30, 4125.00)]
        [TestCase(14, 1925.00)]
        [TestCase(1, 137.50)]
        public void Test_Expenses_Analysis_GetEarnings(int days, decimal expected)
        {
            Assert.That(new Analysis(DateTime.Now, DateTime.Now).GetEarnings(days), Is.EqualTo(expected));
        }
    }
}
