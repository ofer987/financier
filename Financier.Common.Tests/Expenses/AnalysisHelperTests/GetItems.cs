using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using NUnit.Framework;

using Financier.Common.Expenses;
using Financier.Common.Expenses.Models;

namespace Financier.Common.Tests.Expenses.AnalysisHelperTests
{
    public class GetItems : Fixture
    {
        public static IEnumerable TestCases
        {
            get
            {
                yield return new TestCaseData(2019, 6, 896308.7M, new[] {
                    "Edith Salary",
                    "Dan Salary",
                    "Federal Childcare Benefit",
                    "IQ",
                    "Fresco",
                    "Ferrari",
                    "Lambo",
                    "Porsche 911"
                });

                yield return new TestCaseData(2019, 7, -2687.55M, new[] {
                    "Dan Salary",
                    "Federal Childcare Benefit",
                    "Golden Star",
                    "Your Community Grocer",
                    "IQ"
                });
            }
        }

        [Test]
        [TestCaseSource(nameof(TestCases))]
        public void Test_Expenses_AnalysisHelper_GetItems(int year, int month, decimal expectedAmount, IEnumerable<string> expectedItems)
        {
            var startAt = new DateTime(year, month, 1);
            var endAt = startAt.AddMonths(1);
            var actual = AnalysisHelper.GetItemListings(startAt, endAt, ItemTypes.Debit);

            var actualItems = actual
                .SelectMany(item => item.Items);

            // foreach (var item in actual)
            // {
            //     Console.WriteLine(item.Description);
            //     Console.WriteLine(item.Tags.Join(", "));
            // }
            Assert.That(actualItems.Select(item => item.Description), Is.EquivalentTo(expectedItems));
            Assert.That(actualItems.Aggregate(0.00M, (result, item) => result + item.Amount), Is.EqualTo(expectedAmount));
        }
    }
}
