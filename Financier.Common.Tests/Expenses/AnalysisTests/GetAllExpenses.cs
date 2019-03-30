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
                yield return new TestCaseData(2019, 6, 897108.7M, new[] {
                    "Edith Salary",
                    "Dan Salary",
                    "IQ",
                    "Fresco",
                    "Ferrari",
                    "Lambo",
                    "Porsche 911"
                });

                yield return new TestCaseData(2019, 7, 112.45M, new[] {
                    "Golden Star",
                    "Your Community Grocer",
                    "IQ"
                });
            }
        }

        [Test]
        [TestCase(2019, 6)]
        public void Test_Expenses_Analysis_GetAllExpenses(int year, int month)
        {
            var startAt = new DateTime(year, month, 1);
            var endAt = startAt.AddMonths(1).AddDays(-1);
            var actual = new Analysis(startAt, endAt).GetAllExpenses();

            foreach (var item in actual)
            {
                Console.WriteLine(item.Description);
                Console.WriteLine(item.Tags.Join(", "));
            }
            var expected = new[] {
                "Edith Salary",
                "Dan Salary",
                "IQ",
                "Fresco",
                "Ferrari",
                "Lambo",
                "Porsche 911"
            };
            Assert.That(actual.Select(item => item.Description), Is.EquivalentTo(expected));
            Assert.That(actual.Aggregate(0.00M, (result, item) => result + item.Amount), Is.EqualTo(897108.7M));
        }
    }
}
