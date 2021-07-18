using System.Linq;
using NUnit.Framework;

using TagModel = Financier.Common.Expenses.Models.Tag;

namespace Financier.Common.Tests.Expenses.Models
{
    public class ItemTests : InitializedDatabaseTests
    {
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