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
                FactoryData.Accounts.Dan.Cards.DanCard.Statements.June.Items.Porsche.ItemId,
                FactoryData.Accounts.Dan.Cards.DanCard.Statements.June.Items.Ferrari.ItemId
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
                FactoryData.Accounts.Dan.Cards.DanCard.Statements.June.Items.Porsche.ItemId
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
                FactoryData.Accounts.Ron.Cards.RonCard.Statements.Crazy.Items.Lambo.ItemId
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
                FactoryData.Accounts.Dan.Cards.Savings.Statements.June.Items.ChildCareBenefit.ItemId,
                FactoryData.Accounts.Dan.Cards.Savings.Statements.June.Items.DanSalary.ItemId,
                FactoryData.Accounts.Dan.Cards.Savings.Statements.July.Items.DanSalary.ItemId,
                FactoryData.Accounts.Dan.Cards.Savings.Statements.July.Items.ChildCareBenefit.ItemId,
                FactoryData.Accounts.Dan.Cards.Savings.Statements.June.Items.EdithSalary.ItemId,
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
                FactoryData.Accounts.Dan.Cards.Savings.Statements.June.Items.ChildCareBenefit.ItemId,
                FactoryData.Accounts.Dan.Cards.Savings.Statements.June.Items.DanSalary.ItemId,
                FactoryData.Accounts.Dan.Cards.Savings.Statements.June.Items.EdithSalary.ItemId,
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
