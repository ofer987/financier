using System;
using System.Linq;
using NUnit.Framework;

using Financier.Common.Expenses.Models;

namespace Financier.Common.Tests.Expenses.Models.ItemQueryTests
{
    public class Query : DatabaseFixture
    {
        [TestCase(
            "Dan",
            new[] { "fun" },
            new[] {
                ModelFactories.DanCard.June.Items.PorscheItemId,
                ModelFactories.DanCard.June.Items.FerrariItemId
            },
            2019,
            5,
            2019,
            7
        )]
        [TestCase(
            "Dan",
            new[] { "fast" },
            new[] {
                ModelFactories.DanCard.June.Items.PorscheItemId
            },
            2019,
            5,
            2019,
            7
        )]
        [TestCase(
            "Ron",
            new[] { "like-a-dog" },
            new[] {
                ModelFactories.RonCard.CrazyStatement.Items.LamboItemId
            },
            2019,
            6,
            2019,
            8
        )]
        [TestCase(
            "Ron",
            new[] { "fast" },
            new string[] { },
            2019,
            7,
            2019,
            8
        )]
        [TestCase(
            "This-person-does-not-exist",
            new string[] { },
            new string[] { },
            2019,
            5,
            2019,
            7
        )]
        public void Test_Expenses_Models_ItemQuery_Query_ForDebits(
            string accountName,
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
            var result = new ItemQuery(accountName, tagNames, fro, to, ItemTypes.Debit).GetResults();

            Assert.That(result.Items, Is.EquivalentTo(expectedItems));
        }

        [TestCase(
            "Dan",
            new[] { "salary" },
            new[] {
                ModelFactories.SavingsCard.June.Items.ChildCareBenefitItemId,
                ModelFactories.SavingsCard.June.Items.DanSalaryItemId,
                ModelFactories.SavingsCard.July.Items.DanSalaryItemId,
                ModelFactories.SavingsCard.July.Items.ChildCareBenefitItemId,
                ModelFactories.SavingsCard.June.Items.EdithSalaryItemId,
            },
            2019,
            6,
            2019,
            8
        )]
        [TestCase(
            "Dan",
            new[] { "salary" },
            new[] {
                ModelFactories.SavingsCard.June.Items.ChildCareBenefitItemId,
                ModelFactories.SavingsCard.June.Items.DanSalaryItemId,
                ModelFactories.SavingsCard.June.Items.EdithSalaryItemId,
            },
            2019,
            6,
            2019,
            7
        )]
        [TestCase(
            "Dan",
            new[] { "internal" },
            new string[] { },
            2019,
            6,
            2019,
            9
        )]
        [TestCase(
            "Dan",
            new[] { "savings" },
            new string[] { },
            2019,
            6,
            2019,
            9
        )]
        public void Test_Expenses_Models_ItemQuery_Query_ForCredits(
            string accountName,
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
            var result = new ItemQuery(accountName, tagNames, fro, to, ItemTypes.Credit).GetResults();

            Assert.That(result.Items, Is.EquivalentTo(expectedItems));
        }
    }
}
