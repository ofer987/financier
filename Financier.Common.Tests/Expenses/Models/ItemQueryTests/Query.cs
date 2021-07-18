using System;
using System.Linq;
using NUnit.Framework;

using Financier.Common.Expenses.Models;
using ItemModel = Financier.Common.Expenses.Models.Item;

namespace Financier.Common.Tests.Expenses.Models.ItemQueryTests
{
    public class Query : InitializedDatabaseTests
    {
        [TestCase(
            "Dan",
            new[] { FactoryData.Tags.Fun.Name },
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
            new[] { FactoryData.Tags.Fast.Name },
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
            new[] { FactoryData.Tags.Dog.Name },
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
            new[] { FactoryData.Tags.Fast.Name },
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

            var expectedItems = expectedItemIds.Select(itemId => ItemModel.GetByItemId(itemId));
            var result = new ItemQuery(accountName, tagNames, fro, to, ItemTypes.Debit).GetResults();

            Assert.That(result.Items, Is.EquivalentTo(expectedItems));
        }

        [TestCase(
            "Dan",
            new[] { FactoryData.Tags.Salary.Name },
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
            new[] { FactoryData.Tags.Salary.Name },
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
            new[] { FactoryData.Tags.Internal.Name },
            new string[] { },
            2019,
            6,
            2019,
            9
        )]
        [TestCase(
            "Dan",
            new[] { FactoryData.Tags.Savings.Name },
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

            var expectedItems = expectedItemIds.Select(itemId => ItemModel.GetByItemId(itemId));
            var result = new ItemQuery(accountName, tagNames, fro, to, ItemTypes.Credit).GetResults();

            Assert.That(result.Items, Is.EquivalentTo(expectedItems));
        }
    }
}
