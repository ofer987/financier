using System;
using System.Linq;
using NUnit.Framework;

using Financier.Common.Expenses.Models;
using Financier.Common.Expenses;

namespace Financier.Common.Tests.Expenses.AnalysisTests
{
    public class GetItemsByTags : Fixture
    {
        [TestCase(
            new[] { "fun" },
            new[] {
                MyFactories.DanCard.June.Items.PorscheItemId,
                MyFactories.DanCard.June.Items.FerrariItemId
            },
            2019,
            5,
            2019,
            7
        )]
        [TestCase(
            new[] { "fast" },
            new[] {
                MyFactories.DanCard.June.Items.PorscheItemId,
                MyFactories.RonCard.CrazyStatement.Items.LamboItemId
            },
            2019,
            5,
            2019,
            7
        )]
        [TestCase(
            new[] { "fast" },
            new string[] { },
            2019,
            7,
            2019,
            8
        )]
        [TestCase(
            new string[] { },
            new string[] { },
            2019,
            5,
            2019,
            7
        )]
        public void Test_Expenses_Analysis_GetItemsByTags_ForDebits(
            string[] tagNames,
            string[] expectedItemIds,
            int yearFrom,
            int monthFrom,
            int yearTo,
            int monthTo
        )
        {
            DateTime fro = new DateTime(yearFrom, monthFrom, 1);
            DateTime to = new DateTime(yearTo, monthTo, 1);

            var expectedItems = expectedItemIds.Select(itemId => Item.GetByItemId(itemId));
            var actualItems = new Analysis(fro, to).GetItemsByTags(false, tagNames);

            Assert.That(actualItems, Is.EquivalentTo(expectedItems));
        }

        [TestCase(
            new[] { "salary" },
            new[] {
                MyFactories.SavingsCard.June.Items.ChildCareBenefitItemId,
                MyFactories.SavingsCard.June.Items.DanSalaryItemId,
                MyFactories.SavingsCard.June.Items.EdithSalaryItemId,
            },
            2019,
            6,
            2019,
            7
        )]
        [TestCase(
            new[] { "salary" },
            new[] {
                MyFactories.SavingsCard.June.Items.ChildCareBenefitItemId,
                MyFactories.SavingsCard.June.Items.DanSalaryItemId,
                MyFactories.SavingsCard.July.Items.ChildCareBenefitItemId,
                MyFactories.SavingsCard.June.Items.EdithSalaryItemId,
            },
            2019,
            6,
            2019,
            8
        )]
        [TestCase(
            new[] { "internal" },
            new string[] { },
            2019,
            6,
            2019,
            9
        )]
        [TestCase(
            new[] { "savings" },
            new string[] { },
            2019,
            6,
            2019,
            9
        )]
        public void Test_Expenses_Analysis_GetItemsByTags_ForCredits(
            string[] tagNames,
            string[] expectedItemIds,
            int yearFrom,
            int monthFrom,
            int yearTo,
            int monthTo
        )
        {
            DateTime fro = new DateTime(yearFrom, monthFrom, 1);
            DateTime to = new DateTime(yearTo, monthTo, 1);

            var expectedItems = expectedItemIds.Select(itemId => Item.GetByItemId(itemId));
            var actualItems = new Analysis(fro, to).GetItemsByTags(true, tagNames);

            foreach (var item in expectedItems)
            {
                Console.WriteLine(item);
            }
            Assert.That(actualItems, Is.EquivalentTo(expectedItems));
        }
    }
}
