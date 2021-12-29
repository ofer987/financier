using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using NUnit.Framework;

using Financier.Common.Expenses;
using Financier.Common.Expenses.Models;

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

        public static IEnumerable CreditProjectionCases
        {
            get
            {
                yield return new TestCaseData(
                    new DateTime(2019, 7, 1),
                    new DateTime(2019, 8, 1),
                    new DateTime(2019, 9, 1),
                    2800.00M
                );

                yield return new TestCaseData(
                    new DateTime(2019, 7, 1),
                    new DateTime(2019, 10, 1),
                    new DateTime(2019, 11, 1),
                    (1000000M + 2000M + 800M) / 3
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
                    new DateTime(2019, 10, 1)
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

        public static IEnumerable DebitProjectionCases
        {
            get
            {
                yield return new TestCaseData(
                    new DateTime(2019, 7, 1),
                    new DateTime(2019, 8, 1),
                    new DateTime(2019, 9, 1),
                    98.25M + 4.20M + 10.00M
                );

                yield return new TestCaseData(
                    new DateTime(2019, 7, 1),
                    new DateTime(2019, 10, 1),
                    new DateTime(2019, 11, 1),
                    (98.25M + 4.20M + 10.00M) / 3
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
        [TestCaseSource(nameof(CreditProjectionCases))]
        public void Test_Expenses_ProjectedCashFlow_GetProjectedCreditAt(
            DateTime startAt,
            DateTime endAt,
            DateTime projectedAt,
            decimal expectedAmount)
        {
            var cashFlow = new ProjectedCashFlow(startAt, endAt);

            Assert.That(cashFlow.GetProjectedMonthlyListing(projectedAt.Year, projectedAt.Month).CreditAmount, Is.EqualTo(expectedAmount));
        }

        [Test]
        [TestCaseSource(nameof(FutureProjectionFailCases))]
        public void Test_Expenses_ProjectedCashFlow_GetProjectedCreditAt_Fails(
            DateTime startAt,
            DateTime endAt,
            DateTime projectedAt)
        {
            var cashFlow = new ProjectedCashFlow(startAt, endAt);

            Assert.That(() => cashFlow.GetProjectedMonthlyListing(projectedAt.Year, projectedAt.Month), Throws.ArgumentException);
            Assert.That(() => cashFlow.GetProjectedMonthlyListing(projectedAt.Year, projectedAt.Month), (Throws.ArgumentException));
        }

        [Test]
        [TestCaseSource(nameof(DebitProjectionCases))]
        public void Test_Expenses_ProjectedCashFlow_GetProjectedDebitAt(
            DateTime startAt,
            DateTime endAt,
            DateTime projectedAt,
            decimal expectedAmount)
        {
            var cashFlow = new ProjectedCashFlow(startAt, endAt);

            Assert.That(cashFlow.GetProjectedMonthlyListing(projectedAt.Year, projectedAt.Month).DebitAmount, Is.EqualTo(expectedAmount));
        }
    }
}
