using System;
using System.Collections;
using System.Linq;
using NUnit.Framework;

using Financier.Common.Models;
using Financier.Common.Liabilities;

namespace Financier.Common.Tests.Models
{
    public class HomeTests
    {
        public static decimal MonthlyMaintenanceFeesAtPurchasePrice = 300.00M;
        public static DateTime PurchasedAt = new DateTime(2016, 2, 29, 13, 0, 0);

        private static IEnumerable GetMaintenancePaymentsCases()
        {
            yield return new TestCaseData(
                new DateTime(2016, 2, 28, 0, 0, 0),
                0.00M
            );

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
                new DateTime(2016, 3, 29, 13, 0, 0),
                600.00M
            );

            yield return new TestCaseData(
                new DateTime(2017, 2, 27),
                12 * 300.00M
            );

            yield return new TestCaseData(
                new DateTime(2017, 2, 28),
                12 * 300.00M + 306.00M
            );

            yield return new TestCaseData(
                new DateTime(2017, 2, 28, 13, 0, 0),
                12 * 300.00M + 306.00M
            );
        }

        public Home Subject { get; private set; }

        [SetUp]
        public void SetUp()
        {
            var purchasePrice = 1000000.00M;
            var downPayment = 300000.00M;
            var interestRate = 0.03M;
            var mortgage = Mortgages.GetFixedRateMortgage(purchasePrice, interestRate, 300, PurchasedAt, downPayment);

            Subject = new Home("foobar", PurchasedAt, purchasePrice, 300.00M, mortgage, MonthlyMaintenanceFeesAtPurchasePrice);
        }

        [TestCaseSource(nameof(GetMaintenancePaymentsCases))]
        public void Test_Home_GetMaintenancePayments(DateTime at, decimal expected)
        {
            Assert.That(Subject.GetMaintenancePayments(at).Sum(), Is.EqualTo(expected));
        }
    }
}
