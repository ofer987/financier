using System;
using System.Collections;
using NUnit.Framework;

using Financier.Common.Models;

namespace Financier.Common.Tests.Models
{
    public class CompoundYearlyInflationTests
    {
        CompoundYearlyInflation Subject { get; }

        public CompoundYearlyInflationTests()
        {
            Subject = new CompoundYearlyInflation();
        }

        private static IEnumerable GetValueAtTestCases()
        {
            yield return new TestCaseData(
                1000.00M,
                new DateTime(2020, 1, 1),
                new DateTime(2021, 1, 1)
            ).Returns(1020.00M);

            yield return new TestCaseData(
                1000.00M,
                new DateTime(2020, 1, 1),
                new DateTime(2022, 1, 1)
            ).Returns(1040.40M);

            yield return new TestCaseData(
                1000.00M,
                new DateTime(2020, 1, 1),
                new DateTime(2020, 12, 1)
            ).Returns(1000.00M);

            yield return new TestCaseData(
                1000.00M,
                new DateTime(2020, 1, 1),
                new DateTime(2019, 1, 1)
            ).Returns(980.39M);
        }

        [TestCaseSource(nameof(GetValueAtTestCases))]
        public decimal TestGetValueAt(decimal sourceValue, DateTime sourceAt, DateTime targetAt)
        {
            return new Money(sourceValue, sourceAt)
                .GetValueAt(Subject, targetAt);
        }
    }
}
