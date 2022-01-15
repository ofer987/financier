using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

using Financier.Common.Expenses.Models;
using TagModel = Financier.Common.Expenses.Models.Tag;

namespace Financier.Common.Tests.Expenses.Models
{
    public class ItemTests : InitializedDatabaseTests
    {
        public static IEnumerable GetAllByTestCases
        {
            get
            {
                yield return new TestCaseData(
                    new DateTime(2019, 5, 1),
                    new DateTime(2019, 7, 1),
                    Enumerable.Empty<string>(),
                    Enumerable.Empty<string>()
                );

                yield return new TestCaseData(
                    new DateTime(2019, 5, 1),
                    new DateTime(2019, 8, 1),
                    new[] { "credit-card-payment", "internal" },
                    new[] {
                        FactoryData.Accounts.Dan.Cards.DanCard.Statements.June.Items.CreditCardPayment.ItemId,
                        FactoryData.Accounts.Dan.Cards.DanCard.Statements.July.Items.CreditCardPayment.ItemId,
                        FactoryData.Accounts.Ron.Cards.RonCard.Statements.Crazy.Items.CreditCardPayment.ItemId,
                        FactoryData.Accounts.Dan.Cards.Savings.Statements.June.Items.DanCreditCardPayment.ItemId,
                        FactoryData.Accounts.Dan.Cards.Savings.Statements.June.Items.CrazyCreditCardPayment.ItemId,
                        FactoryData.Accounts.Dan.Cards.Savings.Statements.July.Items.DanCreditCardPayment.ItemId,
                    }
                );

                yield return new TestCaseData(
                    new DateTime(2019, 5, 1),
                    new DateTime(2019, 7, 1),
                    new[] { "salary" },
                    new[] {
                        FactoryData.Accounts.Dan.Cards.Savings.Statements.June.Items.DanSalary.ItemId,
                        FactoryData.Accounts.Dan.Cards.Savings.Statements.June.Items.EdithSalary.ItemId,
                        FactoryData.Accounts.Dan.Cards.Savings.Statements.June.Items.ChildCareBenefit.ItemId
                    }
                );
            }
        }

        public static IEnumerable TagNameTestCases
        {
            get
            {
                yield return new TestCaseData(
                    new DateTime(2019, 5, 1),
                    new DateTime(2019, 7, 1),
                    Enumerable.Empty<string>(),
                    Enumerable.Empty<string>()
                );

                yield return new TestCaseData(
                    new DateTime(2019, 5, 1),
                    new DateTime(2019, 8, 1),
                    new[] { "internal" },
                    Enumerable.Empty<string>()
                );

                yield return new TestCaseData(
                    new DateTime(2019, 5, 1),
                    new DateTime(2019, 7, 1),
                    new[] { "salary" },
                    new[] {
                        FactoryData.Accounts.Dan.Cards.Savings.Statements.June.Items.DanSalary.ItemId,
                        FactoryData.Accounts.Dan.Cards.Savings.Statements.June.Items.EdithSalary.ItemId,
                        FactoryData.Accounts.Dan.Cards.Savings.Statements.June.Items.ChildCareBenefit.ItemId
                    }
                );

                yield return new TestCaseData(
                    new DateTime(2019, 5, 1),
                    new DateTime(2019, 8, 1),
                    new[] { "salary" },
                    new[] {
                        FactoryData.Accounts.Dan.Cards.Savings.Statements.June.Items.DanSalary.ItemId,
                        FactoryData.Accounts.Dan.Cards.Savings.Statements.June.Items.EdithSalary.ItemId,
                        FactoryData.Accounts.Dan.Cards.Savings.Statements.June.Items.ChildCareBenefit.ItemId,
                        FactoryData.Accounts.Dan.Cards.Savings.Statements.July.Items.DanSalary.ItemId,
                        FactoryData.Accounts.Dan.Cards.Savings.Statements.July.Items.ChildCareBenefit.ItemId
                    }
                );

                yield return new TestCaseData(
                    new DateTime(2019, 7, 1),
                    new DateTime(2019, 8, 1),
                    new[] { "salary" },
                    new[] {
                        FactoryData.Accounts.Dan.Cards.Savings.Statements.July.Items.DanSalary.ItemId,
                        FactoryData.Accounts.Dan.Cards.Savings.Statements.July.Items.ChildCareBenefit.ItemId
                    }
                );

                yield return new TestCaseData(
                    new DateTime(2019, 5, 1),
                    new DateTime(2019, 7, 1),
                    new[] { "fun", "fast" },
                    new[] {
                        FactoryData.Accounts.Dan.Cards.DanCard.Statements.June.Items.Porsche.ItemId
                    }
                );

                yield return new TestCaseData(
                    new DateTime(2019, 5, 1),
                    new DateTime(2019, 7, 1),
                    new[] { "fun" },
                    new[] {
                        FactoryData.Accounts.Dan.Cards.DanCard.Statements.June.Items.Ferrari.ItemId
                    }
                );

                yield return new TestCaseData(
                    new DateTime(2019, 7, 1),
                    new DateTime(2019, 10, 1),
                    new[] { "fun", "fast" },
                    Enumerable.Empty<string>()
                );

                yield return new TestCaseData(
                    new DateTime(2019, 7, 1),
                    new DateTime(2019, 8, 1),
                    new[] { "salary" },
                    new[] {
                        FactoryData.Accounts.Dan.Cards.Savings.Statements.July.Items.DanSalary.ItemId,
                        FactoryData.Accounts.Dan.Cards.Savings.Statements.July.Items.ChildCareBenefit.ItemId
                    }
                );

                yield return new TestCaseData(
                    new DateTime(2019, 7, 1),
                    new DateTime(2019, 8, 1),
                    new[] { "salary", "coffee" },
                    Enumerable.Empty<string>()
                );
            }
        }

        [Test]
        [TestCaseSource(nameof(GetAllByTestCases))]
        public void Test_Item_GetAllBy(DateTime fromDate, DateTime toDate, IEnumerable<string> tagNames, IEnumerable<string> expected)
        {
            var actual = Item.GetAllBy(fromDate, toDate, tagNames)
                .Select(item => item.ItemId);

            Assert.That(actual, Is.EquivalentTo(expected));
        }

        [Test]
        [TestCaseSource(nameof(TagNameTestCases))]
        public void Test_Item_GetBy(DateTime fromDate, DateTime toDate, IEnumerable<string> tagNames, IEnumerable<string> expected)
        {
            var actual = Item.GetBy(fromDate, toDate, tagNames)
                .Select(item => item.ItemId);

            Assert.That(actual, Is.EquivalentTo(expected));
        }

        [Test]
        [TestCase(
            FactoryData.Accounts.Dan.Cards.DanCard.Statements.June.Items.Ferrari.ItemId,
            FactoryData.Tags.Coffee.Name + ", " + FactoryData.Tags.Fast.Name + "," + FactoryData.Tags.Lunch.Name,
            new string[] { FactoryData.Tags.Coffee.Name, FactoryData.Tags.Fast.Name, FactoryData.Tags.Lunch.Name }
        )]
        [TestCase(
            FactoryData.Accounts.Ron.Cards.RonCard.Statements.Crazy.Items.CreditCardPayment.ItemId,
            FactoryData.Tags.Coffee.Name + "," + FactoryData.Tags.Fast.Name + " , " + FactoryData.Tags.Lunch.Name + " , " + FactoryData.Tags.Coffee.Name,
            new string[] { FactoryData.Tags.Coffee.Name, FactoryData.Tags.Fast.Name, FactoryData.Tags.Lunch.Name }
        )]
        [TestCase(
            FactoryData.Accounts.Ron.Cards.RonCard.Statements.Crazy.Items.CreditCardPayment.ItemId,
            " , , , ",
            new string[] { })]
        public void ItemTest_AddTags_CommaSeparatedValues(string itemId, string commaSeparatedNewTagNames, string[] expectedTagNames)
        {
            var item = Financier.Common.Expenses.Models.Item.GetByItemId(itemId);
            var previousTagNames = (item.Tags ?? Enumerable.Empty<TagModel>())
                .Select(tag => tag.Name)
                .Select(t => t.ToLower());

            item.AddTags(commaSeparatedNewTagNames);

            using (var db = new Context())
            {
                var actualTags = (Financier.Common.Expenses.Models.Item.GetByItemId(itemId).Tags ?? Enumerable.Empty<TagModel>())
                    .Select(t => t.Name);

                CollectionAssert.AreEquivalent(
                    previousTagNames
                        .Union(expectedTagNames.Select(t => t.ToLower()))
                        .Distinct(),
                    actualTags
                );
            }
        }

        [Test]
        [TestCase(FactoryData.Accounts.Ron.Cards.RonCard.Statements.Crazy.Items.CreditCardPayment.ItemId, new string[] { FactoryData.Tags.Coffee.Name, FactoryData.Tags.Coffee.Name }, new string[] { FactoryData.Tags.Coffee.Name })]
        public void ItemTest_AddTags_NoDuplicates(string itemId, string[] newTagNames, string[] expectedNewTagNames)
        {
            var item = Financier.Common.Expenses.Models.Item.GetByItemId(itemId);
            var previousTagNames = (item.Tags ?? Enumerable.Empty<TagModel>())
                .Select(tag => tag.Name)
                .Select(t => t.ToLower());

            item.AddTags(newTagNames);

            using (var db = new Context())
            {
                var actualTags = (Financier.Common.Expenses.Models.Item.GetByItemId(itemId).Tags ?? Enumerable.Empty<TagModel>())
                    .Select(t => t.Name);

                CollectionAssert.AreEquivalent(
                    previousTagNames
                        .Union(expectedNewTagNames.Select(t => t.ToLower()))
                        .Distinct(),
                    actualTags
                );
            }
        }
        [Test]
        [TestCase(FactoryData.Accounts.Dan.Cards.DanCard.Statements.June.Items.Ferrari.ItemId, new string[] { FactoryData.Tags.Coffee.Name, FactoryData.Tags.Fast.Name, FactoryData.Tags.Lunch.Name })]
        [TestCase(FactoryData.Accounts.Ron.Cards.RonCard.Statements.Crazy.Items.CreditCardPayment.ItemId, new string[] { FactoryData.Tags.Coffee.Name, FactoryData.Tags.Fast.Name, FactoryData.Tags.Lunch.Name })]
        [TestCase(FactoryData.Accounts.Ron.Cards.RonCard.Statements.Crazy.Items.CreditCardPayment.ItemId, new string[] { })]
        public void ItemTest_AddTags(string itemId, string[] newTagNames)
        {
            var item = Financier.Common.Expenses.Models.Item.GetByItemId(itemId);
            var previousTagNames = (item.Tags ?? Enumerable.Empty<TagModel>())
                .Select(tag => tag.Name)
                .Select(t => t.ToLower());

            item.AddTags(newTagNames);

            using (var db = new Context())
            {
                var actualTags = (Financier.Common.Expenses.Models.Item.GetByItemId(itemId).Tags ?? Enumerable.Empty<TagModel>())
                    .Select(t => t.Name);

                CollectionAssert.AreEquivalent(
                    previousTagNames
                        .Union(newTagNames.Select(t => t.ToLower()))
                        .Distinct(),
                    actualTags
                );
            }
        }

        [Test]
        [TestCase(
            FactoryData.Accounts.Dan.Cards.DanCard.Statements.June.Items.Ferrari.ItemId,
            new string[] {
                FactoryData.Tags.Coffee.Name,
                FactoryData.Tags.Fast.Name,
                FactoryData.Tags.Fast.Name
            },
            new string[] {
                FactoryData.Tags.Coffee.Name,
                FactoryData.Tags.Fast.Name
            }
        )]
        public void ItemTest_UpdateTags_NoDuplicates(string itemId, string[] newTagNames, string[] expectedTagNames)
        {
            var item = Financier.Common.Expenses.Models.Item.GetByItemId(itemId);

            item.UpdateTags(newTagNames);

            using (var db = new Context())
            {
                var actualTags = (Financier.Common.Expenses.Models.Item.GetByItemId(itemId).Tags ?? Enumerable.Empty<TagModel>())
                    .Select(t => t.Name);

                CollectionAssert.AreEquivalent(
                    expectedTagNames.Select(t => t.ToLower()),
                    actualTags
                );
            }
        }

        [Test]
        [TestCase(FactoryData.Accounts.Dan.Cards.DanCard.Statements.June.Items.Ferrari.ItemId, new string[] { FactoryData.Tags.Coffee.Name, FactoryData.Tags.Fast.Name, FactoryData.Tags.Lunch.Name })]
        [TestCase(FactoryData.Accounts.Ron.Cards.RonCard.Statements.Crazy.Items.CreditCardPayment.ItemId, new string[] { FactoryData.Tags.Coffee.Name, FactoryData.Tags.Fast.Name, FactoryData.Tags.Lunch.Name })]
        [TestCase(FactoryData.Accounts.Ron.Cards.RonCard.Statements.Crazy.Items.CreditCardPayment.ItemId, new string[] { })]
        public void ItemTest_UpdateTags(string itemId, string[] expectedTagNames)
        {
            var item = Financier.Common.Expenses.Models.Item.GetByItemId(itemId);

            item.UpdateTags(expectedTagNames);

            using (var db = new Context())
            {
                var actualTags = (Financier.Common.Expenses.Models.Item.GetByItemId(itemId).Tags ?? Enumerable.Empty<TagModel>())
                    .Select(t => t.Name);

                CollectionAssert.AreEquivalent(
                    expectedTagNames.Select(t => t.ToLower()),
                    actualTags
                );
            }
        }
    }
}
