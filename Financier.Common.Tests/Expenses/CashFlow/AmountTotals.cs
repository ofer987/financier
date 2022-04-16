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
             * For Dan:
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
             *
             * For Ron:
             * June:
             *  Salaries:
             *  Expenses:
             *    300,000.00
             */

            get
            {
                yield return new TestCaseData("Dan", 2019, 6, 2019, 7, 3800.00M, 600108.7M, -19876.96M);
                yield return new TestCaseData("Dan", 2019, 6, 2019, 9, 6600.00M, 600221.15M, -6452.40M);
                yield return new TestCaseData("Dan", 2019, 6, 2019, 11, 1006600.00M, 600221.15M, 2656.07M);
                yield return new TestCaseData("Dan", 2019, 5, 2019, 9, 6600.00M, 600221.15M, -4826.19M);
                yield return new TestCaseData("Ron", 2019, 5, 2019, 6, 0.00M, 0.00M, 0.00M);
                yield return new TestCaseData("Ron", 2019, 6, 2019, 7, 0.00M, 300000.00M, -10000.00M);
            }
        }

        [TestCaseSource(nameof(TestCases))]
        public void Test_Expenses_CashFlow_AmountTotals(
            string accountName,
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
            var cashFlow = new DurationCashFlow(accountName, startAt, endAt);

            Assert.That(cashFlow.CreditAmountTotal, Is.EqualTo(expectedCreditAmount));
            Assert.That(cashFlow.DebitAmountTotal, Is.EqualTo(expectedDebitAmount));
            Assert.That(cashFlow.ProfitAmountTotal, Is.EqualTo(expectedCreditAmount - expectedDebitAmount));
            Assert.That(cashFlow.DailyProfit, Is.EqualTo(expectedDailyProfit));
        }
    }
}
