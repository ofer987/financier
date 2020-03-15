using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

using Financier.Common.Liabilities;

namespace Financier.Common.Tests.Liabilities
{
    public class CappedPaymentsTest
    {
        public CappedPayments Subject { get; private set; }

        public static IEnumerable GetRangePositiveTestCases
        {
            get
            {
                yield return new TestCaseData(
                    20000.00M,
                    new[] {
                        ValueTuple.Create(new DateTime(2019, 1, 1), 10000.00M),
                        ValueTuple.Create(new DateTime(2019, 2, 1), 10000.00M)
                    },
                    new DateTime(2019, 1, 1),
                    new DateTime(2019, 12, 1),
                    new[] {
                        ValueTuple.Create(new DateTime(2019, 1, 1), 10000.00M),
                        ValueTuple.Create(new DateTime(2019, 2, 1), 10000.00M)
                    }
                );

                yield return new TestCaseData(
                    20000.00M,
                    new[] {
                        ValueTuple.Create(new DateTime(2019, 1, 1), 10000.00M),
                        ValueTuple.Create(new DateTime(2019, 2, 1), 5000.00M)
                    },
                    new DateTime(2019, 1, 1),
                    new DateTime(2019, 12, 1),
                    new[] {
                        ValueTuple.Create(new DateTime(2019, 1, 1), 10000.00M),
                        ValueTuple.Create(new DateTime(2019, 2, 1), 5000.00M)
                    }
                );
            }
        }

        public static IEnumerable GetRangeEmptyTestCases
        {
            get
            {
                yield return new TestCaseData(
                    20000.00M,
                    new DateTime(2019, 1, 1),
                    new DateTime(2019, 12, 1)
                );

                yield return new TestCaseData(
                    100.00M,
                    new DateTime(2019, 1, 1),
                    new DateTime(2019, 12, 1)
                );
            }
        }

        public static IEnumerable GetRangeInvalidTestCases
        {
            get
            {
                yield return new TestCaseData(
                    100.00M,
                    new DateTime(2019, 2, 1),
                    new DateTime(2019, 1, 1),
                    typeof(ArgumentOutOfRangeException)
                );

                yield return new TestCaseData(
                    0.00M,
                    new DateTime(2019, 1, 1),
                    new DateTime(2019, 2, 1),
                    typeof(ArgumentOutOfRangeException)
                );

                yield return new TestCaseData(
                    -100.00M,
                    new DateTime(2019, 1, 1),
                    new DateTime(2019, 2, 1),
                    typeof(ArgumentOutOfRangeException)
                );

            }
        }

        public static IEnumerable AddInvalidTestCases
        {
            get
            {
                yield return new TestCaseData(
                    20000.00M,
                    new DateTime(2019, 1, 1),
                    new DateTime(2019, 12, 1),
                    new[] {
                        ValueTuple.Create(new DateTime(2019, 1, 1), 100000.00M),
                        ValueTuple.Create(new DateTime(2019, 2, 1), 20001.00M)
                    },
                    typeof(OverPaymentException)
                );
            }
        }

        [TestCaseSource(nameof(GetRangePositiveTestCases))]
        public void Test_Add_and_GetRange(decimal total, IEnumerable<ValueTuple<DateTime, decimal>> amounts, DateTime startAt, DateTime endAt, IEnumerable<ValueTuple<DateTime, decimal>> expected)
        {
            Subject = new CappedPayments(total);

            foreach (var amount in amounts)
            {
                Subject.Add(amount.Item1, amount.Item2);
            }

            Assert.That(Subject.GetRange(startAt, endAt), Is.EquivalentTo(expected));
        }

        [TestCaseSource(nameof(GetRangeEmptyTestCases))]
        public void Test_Add_and_GetRange_Empty(decimal total, DateTime startAt, DateTime endAt)
        {
            Subject = new CappedPayments(total);

            Assert.That(Subject.GetRange(startAt, endAt), Is.Empty);
        }

        [TestCaseSource(nameof(GetRangeInvalidTestCases))]
        public void Test_Add_and_GetRange_ThrowsUp(decimal total, DateTime startAt, DateTime endAt, Type expectedExceptionType)
        {
            Assert.Throws(expectedExceptionType, () => new CappedPayments(total).GetRange(startAt, endAt));
        }

        [TestCaseSource(nameof(AddInvalidTestCases))]
        public void Test_Add_ThrowsUp(decimal total, DateTime startAt, DateTime endAt, IEnumerable<ValueTuple<DateTime, decimal>> amounts, Type expectedExceptionType)
        {
            Subject = new CappedPayments(total);

            foreach (var amount in amounts)
            {
                Assert.Throws(expectedExceptionType, () => Subject.Add(amount.Item1, amount.Item2));
            }
        }
    }
}
