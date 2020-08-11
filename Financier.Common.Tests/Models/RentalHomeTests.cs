using System;
using System.Collections;
using System.Linq;
using NUnit.Framework;

using Financier.Common.Models;

namespace Financier.Common.Tests.Models
{
    public class RentalHomeTests
    {
        public static DateTime PurchasedAt = new DateTime(2016, 2, 29, 13, 0, 0);

        private static IEnumerable CostAtCases()
        {
            yield return new TestCaseData(
                new DateTime(2016, 2, 29, 12, 59, 0),
                300.00M
            );

            yield return new TestCaseData(
                new DateTime(2016, 2, 29, 13, 0, 0),
                300.00M
            );

            yield return new TestCaseData(
                new DateTime(2016, 2, 29, 13, 0, 1),
                300.00M
            );

            yield return new TestCaseData(
                new DateTime(2016, 2, 29),
                300.00M
            );

            yield return new TestCaseData(
                new DateTime(2016, 3, 29),
                600.00M
            );

            yield return new TestCaseData(
                new DateTime(2017, 2, 27),
                12 * 300.00M
            );

            yield return new TestCaseData(
                new DateTime(2017, 2, 28),
                13 * 300.00M
            );
        }

        public RentalHome Subject { get; private set; }

        [SetUp]
        public void SetUp()
        {
            Subject = new RentalHome("foobar", PurchasedAt, 300.00M);
        }

        [TestCaseSource(nameof(CostAtCases))]
        public void Test_RentalHome_GetCostAt(DateTime at, decimal expected)
        {
            Assert.That(Subject.GetCostAt(at).Sum(), Is.EqualTo(expected));
        }

        public void Test_RentalHome_GetCostAt_Fails_ForEarlierDates()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Subject.GetCostAt(PurchasedAt.AddDays(-1)));
        }
    }
}
