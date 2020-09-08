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

        [TestCase(2020, 1, 31, 2020, 1, 28)]
        [TestCase(2020, 1, 28, 2020, 1, 28)]
        [TestCase(2020, 2, 28, 2020, 2, 28)]
        [TestCase(2019, 1, 29, 2019, 1, 28)]
        [TestCase(2020, 12, 31, 2020, 12, 28)]
        [TestCase(2010, 12, 31, 2010, 12, 28)]
        public void Test_Constructor(int year, int month, int day, int expectedYear, int expectedMonth, int expectedDay)
        {
            var date = new DateTime(year, month, day);
            var money = new Money(0.00M, date);
            Assert.That(money.At.Year, Is.EqualTo(expectedYear));
            Assert.That(money.At.Month, Is.EqualTo(expectedMonth));
            Assert.That(money.At.Day, Is.EqualTo(expectedDay));
        }
    }
}
