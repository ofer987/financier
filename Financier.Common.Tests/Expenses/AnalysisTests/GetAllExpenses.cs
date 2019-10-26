using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using NUnit.Framework;

using Financier.Common.Expenses;
using Financier.Common.Extensions;

namespace Financier.Common.Tests.Expenses.AnalysisTests
{
    public class GetAllExpenses : Fixture
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
        public void Test_Expenses_Analysis_GetAllExpenses(int year, int month, decimal expectedAmount, IEnumerable<string> expectedItems)
        {
            var startAt = new DateTime(year, month, 1);
            var endAt = startAt.AddMonths(1).AddDays(-1);
            var actual = new Analysis(startAt, endAt).GetAllExpenses();

            foreach (var item in actual)
            {
                Console.WriteLine(item.Description);
                Console.WriteLine(item.Tags.Join(", "));
            }
            Assert.That(actual.Select(item => item.Description), Is.EquivalentTo(expectedItems));
            Assert.That(actual.Aggregate(0.00M, (result, item) => result + item.TheRealAmount), Is.EqualTo(expectedAmount));
        }
    }
}
