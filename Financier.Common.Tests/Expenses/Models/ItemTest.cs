using System;
using System.Linq;
using NUnit.Framework;

using Financier.Common.Expenses.Models;

namespace Financier.Common.Tests.Expenses.Models
{
    public class ItemTest : InitializedDatabaseTests
    {
        [Test]
        [TestCase(FactoryData.Accounts.Dan.Cards.DanCard.Statements.June.Items.Ferrari.ItemId, new string[] { FactoryData.Tags.Coffee.Name, FactoryData.Tags.Fast.Name, FactoryData.Tags.Lunch.Name })]
        [TestCase(FactoryData.Accounts.Ron.Cards.RonCard.Statements.Crazy.Items.CreditCardPayment.ItemId, new string[] { FactoryData.Tags.Coffee.Name, FactoryData.Tags.Fast.Name, FactoryData.Tags.Lunch.Name })]
        [TestCase(FactoryData.Accounts.Ron.Cards.RonCard.Statements.Crazy.Items.CreditCardPayment.ItemId, new string[] { })]
        public void ItemTest_AddTags(string itemId, string[] newTagNames)
        {
            var item = Item.GetByItemId(itemId);
            var previousTagNames = (item.Tags ?? Enumerable.Empty<Tag>())
                .Select(tag => tag.Name)
                .Select(t => t.ToLower());

            item.AddTags(newTagNames);

            using (var db = new Context())
            {
                var actualTags = (Item.GetByItemId(itemId).Tags ?? Enumerable.Empty<Tag>())
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
        [TestCase(FactoryData.Accounts.Dan.Cards.DanCard.Statements.June.Items.Ferrari.ItemId, new string[] { FactoryData.Tags.Coffee.Name, FactoryData.Tags.Fast.Name, FactoryData.Tags.Lunch.Name })]
        [TestCase(FactoryData.Accounts.Ron.Cards.RonCard.Statements.Crazy.Items.CreditCardPayment.ItemId, new string[] { FactoryData.Tags.Coffee.Name, FactoryData.Tags.Fast.Name, FactoryData.Tags.Lunch.Name })]
        [TestCase(FactoryData.Accounts.Ron.Cards.RonCard.Statements.Crazy.Items.CreditCardPayment.ItemId, new string[] { })]
        public void ItemTest_UpdateTags(string itemId, string[] expectedTagNames)
        {
            var item = Item.GetByItemId(itemId);

            item.UpdateTags(expectedTagNames);

            using (var db = new Context())
            {
                var actualTags = (Item.GetByItemId(itemId).Tags ?? Enumerable.Empty<Tag>())
                    .Select(t => t.Name);

                CollectionAssert.AreEquivalent(
                    expectedTagNames.Select(t => t.ToLower()),
                    actualTags
                );
            }
        }
    }
}
