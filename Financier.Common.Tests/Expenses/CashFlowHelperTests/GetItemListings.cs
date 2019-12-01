using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using NUnit.Framework;

using Financier.Common.Expenses;
using Financier.Common.Expenses.Models;

namespace Financier.Common.Tests.Expenses.CashFlowHelperTests
{
    public class GetItemListings : Fixture
    {
        public static IEnumerable DebitTestCases
        {
            get
            {
                yield return new TestCaseData(2019, 6, ItemTypes.Debit, 900108.70M, new[] {
                    "IQ",
                    "Fresco",
                    "Ferrari",
                    "Lambo",
                    "Porsche 911"
                });

                yield return new TestCaseData(2019, 7, ItemTypes.Debit, 112.45M, new[] {
                    "Golden Star",
                    "Your Community Grocer",
                    "IQ"
                });
            }
        }

        public static IEnumerable CreditTestCases
        {
            get
            {
                yield return new TestCaseData(2019, 6, ItemTypes.Credit, -3800.00M, new[] {
                    "Edith Salary",
                    "Dan Salary",
                    "Federal Childcare Benefit",
                });

                yield return new TestCaseData(2019, 7, ItemTypes.Credit, -2800.00M, new[] {
                    "Dan Salary",
                    "Federal Childcare Benefit",
                });
            }
        }

        [Test]
        [TestCaseSource(nameof(DebitTestCases))]
        [TestCaseSource(nameof(CreditTestCases))]
        public void Test_Expenses_CashFlowHelper_GetItemListings(
            int year,
            int month,
            ItemTypes type,
            decimal expectedAmount,
            IEnumerable<string> expectedItems)
        {
            var startAt = new DateTime(year, month, 1);
            var endAt = startAt.AddMonths(1);
            var actual = CashFlowHelper.GetItemListings(startAt, endAt, type);

            var actualItems = actual
                .SelectMany(item => item.Items);

            Assert.That(actualItems.Select(item => item.Description), Is.EquivalentTo(expectedItems));
            Assert.That(actualItems.Aggregate(0.00M, (result, item) => result + item.Amount), Is.EqualTo(expectedAmount));
        }
    }
}
