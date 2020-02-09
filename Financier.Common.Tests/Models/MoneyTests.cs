using System;
using System.Collections;
using NUnit.Framework;

using Financier.Common.Models;

namespace Financier.Common.Tests.Models
{
    public class MoneyTests
    {
        private static IEnumerable PositiveIncrementOperatorTestCases()
        {
            yield return new TestCaseData(
                100.00M,
                new DateTime(2020, 1, 1),
                50.00M,
                new DateTime(2020, 1, 1),
                150.00M,
                new DateTime(2020, 1, 1)
            );

            yield return new TestCaseData(
                50.00M,
                new DateTime(2020, 1, 1),
                100.00M,
                new DateTime(2020, 1, 1),
                150.00M,
                new DateTime(2020, 1, 1)
            );
        }

        private static IEnumerable NegativeIncrementOperatorTestCases()
        {
            yield return new TestCaseData(
                100.00M,
                new DateTime(2020, 1, 1),
                50.00M,
                new DateTime(2021, 1, 1)
            );

            yield return new TestCaseData(
                50.00M,
                new DateTime(2020, 1, 1),
                100.00M,
                new DateTime(2020, 1, 2)
            );
        }

        private static IEnumerable PositiveDecrementOperatorTestCases()
        {
            yield return new TestCaseData(
                100.00M,
                new DateTime(2020, 1, 1),
                50.00M,
                new DateTime(2020, 1, 1),
                50.00M,
                new DateTime(2020, 1, 1)
            );

            yield return new TestCaseData(
                50.00M,
                new DateTime(2020, 1, 1),
                100.00M,
                new DateTime(2020, 1, 1),
                -50.00M,
                new DateTime(2020, 1, 1)
            );
        }

        private static IEnumerable NegativeDecrementOperatorTestCases()
        {
            yield return new TestCaseData(
                100.00M,
                new DateTime(2020, 1, 1),
                50.00M,
                new DateTime(2021, 1, 1)
            );

            yield return new TestCaseData(
                50.00M,
                new DateTime(2020, 1, 1),
                100.00M,
                new DateTime(2020, 1, 2)
            );
        }

        [TestCaseSource(nameof(PositiveIncrementOperatorTestCases))]
        public void TestIncrementOperator(decimal sourceValue, DateTime sourceAt, decimal targetValue, DateTime targetAt, decimal expectedValue, DateTime expectedAt)
        {
            var expected = new Money(sourceValue, sourceAt) + new Money(targetValue, targetAt);

            Assert.That(expected.Value, Is.EqualTo(expectedValue));
            Assert.That(expected.At, Is.EqualTo(expectedAt));
        }

        [TestCaseSource(nameof(NegativeIncrementOperatorTestCases))]
        public void TestIncrementOperator(decimal sourceValue, DateTime sourceAt, decimal targetValue, DateTime targetAt)
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                var foo = new Money(sourceValue, sourceAt) + new Money(targetValue, targetAt);
            });
        }
        [TestCaseSource(nameof(PositiveDecrementOperatorTestCases))]
        public void TestDecrementOperator(decimal sourceValue, DateTime sourceAt, decimal targetValue, DateTime targetAt, decimal expectedValue, DateTime expectedAt)
        {
            var expected = new Money(sourceValue, sourceAt) - new Money(targetValue, targetAt);

            Assert.That(expected.Value, Is.EqualTo(expectedValue));
            Assert.That(expected.At, Is.EqualTo(expectedAt));
        }

        [TestCaseSource(nameof(NegativeDecrementOperatorTestCases))]
        public void TestDecrementOperator(decimal sourceValue, DateTime sourceAt, decimal targetValue, DateTime targetAt)
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                var foo = new Money(sourceValue, sourceAt) - new Money(targetValue, targetAt);
            });
        }
    }
}
