using System;
using System.Collections;
using NUnit.Framework;

using Financier.Common.Expenses;

namespace Financier.Common.Tests.Expenses.CashFlowHelperTests
{
    public class AmountTotals : InitializedDatabaseTests
    {
        public static IEnumerable TestCases
        {
            /*
             * June
             *  Salaries:
             *    2,000
             *    1,000
             *    800
             *  Expenses:
             *    104.5
             *    4.20
             *    300,000.00
             *    300,000.00
             *    300,000.00
             * July
             *  Salaries:
             *    2,000
             *    800
             *  Expenses:
             *    98.25
             *    4.20
             *    10
             * October
             *  Salaries:
             *    1,000,000
             */

            get
            {
                yield return new TestCaseData(2019, 6, 2019, 7, 3800.00M, 900108.7M, -29876.96M);
                yield return new TestCaseData(2019, 6, 2019, 9, 6600.00M, 900221.15M, -9713.27M);
                yield return new TestCaseData(2019, 6, 2019, 11, 1006600.00M, 900221.15M, 695.29M);
                yield return new TestCaseData(2019, 5, 2019, 9, 6600.00M, 900221.15M, -7265.21M);
                yield return new TestCaseData(2019, 5, 2019, 6, 0.00M, 0.00M, 0.00M);
            }
        }

        [TestCaseSource(nameof(TestCases))]
        public void Test_Expenses_CashFlow_AmountTotals(
            int fromYear,
            int fromMonth,
            int toYear,
            int toMonth,
            decimal expectedCreditAmount,
            decimal expectedDebitAmount,
            decimal expectedDailyProfit)
        {
            var startAt = new DateTime(fromYear, fromMonth, 1);
            var endAt = new DateTime(toYear, toMonth, 1);
            var cashFlow = new DurationCashFlow("Dan", startAt, endAt);

            Assert.That(cashFlow.CreditAmountTotal, Is.EqualTo(expectedCreditAmount));
            Assert.That(cashFlow.DebitAmountTotal, Is.EqualTo(expectedDebitAmount));
            Assert.That(cashFlow.ProfitAmountTotal, Is.EqualTo(expectedCreditAmount - expectedDebitAmount));
            Assert.That(cashFlow.DailyProfit, Is.EqualTo(expectedDailyProfit));
        }
    }
}
