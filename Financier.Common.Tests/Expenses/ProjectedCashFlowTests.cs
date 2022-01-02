using System;
using System.Collections;
using NUnit.Framework;

using Financier.Common.Expenses;

namespace Financier.Common.Tests.Expenses
{
    public class ProjectedCashFlowTests : InitializedDatabaseTests
    {
        public static IEnumerable ProjectionFailCases
        {
            get
            {
                yield return new TestCaseData(
                    new DateTime(2019, 9, 1),
                    new DateTime(2019, 9, 15),
                    new DateTime(2020, 12, 1)
                );

                yield return new TestCaseData(
                    new DateTime(2019, 9, 1),
                    new DateTime(2019, 9, 15),
                    new DateTime(2020, 10, 1)
                );

                yield return new TestCaseData(
                    new DateTime(2019, 9, 1),
                    new DateTime(2019, 9, 30),
                    new DateTime(2020, 10, 1)
                );
            }
        }

        public static IEnumerable ProjectionCases
        {
            get
            {
                yield return new TestCaseData(
                    new DateTime(2019, 7, 1),
                    new DateTime(2019, 8, 1),
                    new DateTime(2019, 9, 1),
                    2800.00M,
                    98.25M + 4.20M + 10.00M
                );

                yield return new TestCaseData(
                    new DateTime(2019, 7, 1),
                    new DateTime(2019, 10, 1),
                    new DateTime(2019, 11, 1),
                    (1000000M + 2000M + 800M) / 3,
                    (98.25M + 4.20M + 10.00M) / 3
                );
            }
        }

        public static IEnumerable FutureProjectionFailCases
        {
            get
            {
                yield return new TestCaseData(
                    new DateTime(2019, 9, 1),
                    new DateTime(2019, 10, 1),
                    new DateTime(2019, 9, 1)
                );

                yield return new TestCaseData(
                    new DateTime(2019, 7, 1),
                    new DateTime(2019, 10, 1),
                    new DateTime(2019, 8, 1)
                );

                yield return new TestCaseData(
                    new DateTime(2019, 7, 1),
                    new DateTime(2019, 10, 1),
                    new DateTime(2019, 6, 1)
                );
            }
        }

        [Test]
        [TestCaseSource(nameof(ProjectionFailCases))]
        public void Test_Expenses_ProjectedCashFlow_Fails(
            DateTime startAt,
            DateTime endAt,
            DateTime projectedAt)
        {
            Assert.That(() => new ProjectedCashFlow(startAt, endAt), Throws.ArgumentException);
        }

        [Test]
        [TestCaseSource(nameof(ProjectionCases))]
        public void Test_Expenses_ProjectedCashFlow_GetProjectedMonthlyListing(
            DateTime startAt,
            DateTime endAt,
            DateTime projectedAt,
            decimal expectedCreditAmount,
            decimal expectedDebitAmount)
        {
            var cashFlow = new ProjectedCashFlow(startAt, endAt);

            Assert.That(cashFlow.GetProjectedMonthlyListing(projectedAt.Year, projectedAt.Month).IsPrediction, Is.True);
            Assert.That(cashFlow.GetProjectedMonthlyListing(projectedAt.Year, projectedAt.Month).Credit, Is.EqualTo(expectedCreditAmount));
            Assert.That(cashFlow.GetProjectedMonthlyListing(projectedAt.Year, projectedAt.Month).Debit, Is.EqualTo(expectedDebitAmount));
        }

        [Test]
        [TestCaseSource(nameof(FutureProjectionFailCases))]
        public void Test_Expenses_ProjectedCashFlow_GetProjectedMonthlyListing_Fails(
            DateTime startAt,
            DateTime endAt,
            DateTime projectedAt)
        {
            var cashFlow = new ProjectedCashFlow(startAt, endAt);

            Assert.That(() => cashFlow.GetProjectedMonthlyListing(projectedAt.Year, projectedAt.Month), Throws.ArgumentException);
        }
    }
}
