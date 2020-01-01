using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

using Financier.Common.Liabilities;

namespace Financier.Common.Tests.Liabilities
{
    public class PaymentsTest
    {
        public Payments Subject { get; private set; }

        public static IEnumerable GetRangePositiveTestCases
        {
            get
            {
                yield return new TestCaseData(
                    new[] {
                    ValueTuple.Create(new DateTime(2019, 1, 1), 10000),
                    ValueTuple.Create(new DateTime(2019, 2, 1), 10000)
                    },
                    new DateTime(2019, 1, 1),
                    new DateTime(2019, 12, 1),
                    20000
                );

                yield return new TestCaseData(
                    new[] {
                    ValueTuple.Create(new DateTime(2019, 1, 1), 10000),
                    ValueTuple.Create(new DateTime(2019, 2, 1), 20000)
                    },
                    new DateTime(2019, 1, 1),
                    new DateTime(2019, 12, 1),
                    30000
                );

                yield return new TestCaseData(
                    new[] {
                        ValueTuple.Create(new DateTime(2019, 1, 1), 10000),
                        ValueTuple.Create(new DateTime(2019, 2, 1), 20000),
                        ValueTuple.Create(new DateTime(2020, 3, 2), 40000)
                        },
                    new DateTime(2019, 1, 1),
                    new DateTime(2019, 12, 1),
                    20000
                );
            }
        }

        public IEnumerable GetRangeEmptyTestCases
        {
            get
            {
                yield return new TestCaseData(
                    new DateTime(2019, 1, 1),
                    new DateTime(2019, 12, 1),
                    new[] {
                    ValueTuple.Create(new DateTime(2019, 1, 1), 10000),
                    ValueTuple.Create(new DateTime(2019, 2, 1), 20000)
                    }
                );

                yield return new TestCaseData(
                    new DateTime(2019, 1, 1),
                    new DateTime(2019, 12, 1),
                    new ValueTuple<DateTime, decimal>[] {
                    }
                );

                yield return new TestCaseData(
                    new DateTime(2019, 1, 1),
                    new DateTime(2019, 12, 1),
                    new ValueTuple<DateTime, decimal>[] {
                    }
                );
            }
        }

        public IEnumerable GetRangeInvalidTestCases
        {
            get
            {
                yield return new TestCaseData(
                    100,
                    new DateTime(2019, 2, 1),
                    new DateTime(2019, 1, 1)
                );
            }
        }

        [TestCaseSource(nameof(GetRangePositiveTestCases))]
        public void Test_Add_and_GetRange(DateTime startAt, DateTime endAt, IEnumerable<ValueTuple<DateTime, decimal>> amounts, IEnumerable<ValueTuple<DateTime, decimal>> expected)
        {
            Subject = new Payments();

            foreach (var amount in amounts)
            {
                Subject.Add(amount.Item1, amount.Item2);
            }

            Assert.That(Subject.GetRange(startAt, endAt), Is.EquivalentTo(expected));
        }

        [TestCaseSource(nameof(GetRangeEmptyTestCases))]
        public void Test_Add_and_GetRange_Empty(DateTime startAt, DateTime endAt, IEnumerable<ValueTuple<DateTime, decimal>> amounts)
        {
            Subject = new Payments();

            foreach (var amount in amounts)
            {
                Subject.Add(amount.Item1, amount.Item2);
            }

            Assert.That(Subject.GetRange(startAt, endAt), Is.Empty);
        }


        [TestCaseSource(nameof(GetRangeInvalidTestCases))]
        public void Test_Add_and_GetRange_ThrowsUp(decimal total, DateTime startAt, DateTime endAt)
        {
            Assert.That(new Payments().GetRange(startAt, endAt), Throws.Exception);
        }
    }
}
