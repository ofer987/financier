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
                    new DateTime(2019, 9, 15)
                );

                yield return new TestCaseData(
                    new DateTime(2019, 9, 1),
                    new DateTime(2019, 9, 30)
                );
            }
        }

        public static IEnumerable ProjectionCases
        {
            get
            {
                yield return new TestCaseData(
                    new DateTime(2019, 5, 1),
                    new DateTime(2019, 8, 1),
                    new DateTime(2019, 9, 1),
                    (2000.00M + 1000.00M + 800.00M + 2800.00M) / 2,
                    (300000.00M + 300000.00M + 300000.00M + 104.50M + 4.20M + 98.25M + 4.20M + 10.00M) / 2
                );

                yield return new TestCaseData(
                    new DateTime(2019, 6, 1),
                    new DateTime(2019, 8, 1),
                    new DateTime(2019, 9, 1),
                    (2000.00M + 1000.00M + 800.00M + 2800.00M) / 2,
                    (300000.00M + 300000.00M + 300000.00M + 104.50M + 4.20M + 98.25M + 4.20M + 10.00M) / 2
                );

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
                    (2000.00M + 800.00M) / 1,
                    (98.25M + 4.20M + 10.00M) / 1
                );

                yield return new TestCaseData(
                    new DateTime(2019, 7, 1),
                    new DateTime(2019, 11, 1),
                    new DateTime(2019, 12, 1),
                    (2000.00M + 800.00M + 1000000.00M) / 4,
                    (98.25M + 4.20M + 10.00M) / 4
                );
            }
        }

        public static IEnumerable ExistingMonthlyCases
        {
            get
            {
                yield return new TestCaseData(
                    new DateTime(2019, 5, 1),
                    new DateTime(2019, 8, 1),
                    new DateTime(2019, 7, 1),
                    2000.00M + 800.00M,
                    10.00M + 98.25M + 4.20M
                );
            }
        }

        public static IEnumerable NullMonthlyCases
        {
            get
            {
                yield return new TestCaseData(
                    new DateTime(2019, 6, 1),
                    new DateTime(2019, 11, 1),
                    new DateTime(2019, 9, 1)
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
                    new DateTime(2019, 6, 1)
                );
            }
        }

        public static IEnumerable StartAtAndEndAtCases
        {
            get
            {
                yield return new TestCaseData(
                    new DateTime(2019, 6, 1),
                    new DateTime(2019, 8, 1),
                    new DateTime(2019, 6, 1),
                    new DateTime(2019, 8, 1)
                );

                yield return new TestCaseData(
                    new DateTime(2019, 5, 1),
                    new DateTime(2019, 8, 1),
                    new DateTime(2019, 6, 1),
                    new DateTime(2019, 8, 1)
                );

                yield return new TestCaseData(
                    new DateTime(2019, 6, 1),
                    new DateTime(2019, 12, 1),
                    new DateTime(2019, 6, 1),
                    new DateTime(2019, 11, 1)
                );

                yield return new TestCaseData(
                    new DateTime(2019, 4, 1),
                    new DateTime(2019, 12, 1),
                    new DateTime(2019, 6, 1),
                    new DateTime(2019, 11, 1)
                );
            }
        }

        [Test]
        [TestCaseSource(nameof(ProjectionFailCases))]
        public void Test_Expenses_ProjectedCashFlow_Fails(
            DateTime startAt,
            DateTime endAt)
        {
            Assert.That(() => new ProjectedCashFlow(startAt, endAt), Throws.ArgumentException);
        }

        [Test]
        [TestCaseSource(nameof(ExistingMonthlyCases))]
        public void Test_Expenses_ProjectedCashFlow_GetExistingMonthlyListing(
            DateTime startAt,
            DateTime endAt,
            DateTime at,
            decimal expectedCreditAmount,
            decimal expectedDebitAmount)
        {
            var cashFlow = new ProjectedCashFlow(startAt, endAt);

            Assert.That(cashFlow.GetMonthlyListing(at.Year, at.Month).IsPrediction, Is.False);
            Assert.That(cashFlow.GetMonthlyListing(at.Year, at.Month).Credit, Is.EqualTo(expectedCreditAmount));
            Assert.That(cashFlow.GetMonthlyListing(at.Year, at.Month).Debit, Is.EqualTo(expectedDebitAmount));
            Assert.That(cashFlow.GetMonthlyListing(at.Year, at.Month).IsNull, Is.False);
        }

        [Test]
        [TestCaseSource(nameof(NullMonthlyCases))]
        public void Test_Expenses_ProjectedCashFlow_GetExistingMonthlyListing_Null(
            DateTime startAt,
            DateTime endAt,
            DateTime at)
        {
            var cashFlow = new ProjectedCashFlow(startAt, endAt);

            Assert.That(cashFlow.GetMonthlyListing(at.Year, at.Month).IsPrediction, Is.False);
            Assert.That(cashFlow.GetMonthlyListing(at.Year, at.Month).IsNull, Is.True);
            Assert.That(cashFlow.GetMonthlyListing(at.Year, at.Month).Credit, Is.EqualTo(0.00M));
            Assert.That(cashFlow.GetMonthlyListing(at.Year, at.Month).Debit, Is.EqualTo(0.00M));
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
            Assert.That(cashFlow.GetProjectedMonthlyListing(projectedAt.Year, projectedAt.Month).IsNull, Is.False);
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

        [Test]
        [TestCaseSource(nameof(StartAtAndEndAtCases))]
        public void Test_Expenses_ProjectedCashFlow_StartAt_EndAt(
            DateTime startAt,
            DateTime endAt,
            DateTime expectedStartAt,
            DateTime expectedEndAt)
        {
            var cashFlow = new ProjectedCashFlow(startAt, endAt);

            Assert.That(() => cashFlow.StartAt, Is.EqualTo(expectedStartAt));
            Assert.That(() => cashFlow.EndAt, Is.EqualTo(expectedEndAt));
        }
    }
}
