using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

using Financier.Common.Expenses.Models;
using TaggingModel = Financier.Common.Expenses.Tagging;

namespace Financier.Common.Tests.Expenses.Tagging
{
    public class AddTagsToItemsTests : DatabaseFixture
    {
        public static IEnumerable<TaggingModel> Taggings;

        [SetUp]
        public void Setup()
        {
            var foodTaggings1 = new TaggingModel("freshco", new[] { "food", "groceries" });
            var foodTaggings2 = new TaggingModel("groceries", new[] { "food", "groceries" });

            var coffeeTaggings = new TaggingModel("meridian", new[] { "coffee" });
            var giftTaggings = new TaggingModel("toys", new[] { "gift" });

            Taggings = new List<TaggingModel>
            {
                foodTaggings1,
                foodTaggings2,
                coffeeTaggings,
                giftTaggings
            };
        }

        [TestCase("FreshCo Bathurst and Steeles", new[] { "food", "groceries" })]
        [TestCase("Bathurst and Steeles freshco", new[] { "food", "groceries" })]
        [TestCase("groceries bodega", new[] { "food", "groceries" })]
        [TestCase("meridian i. i. c place", new[] { "coffee" })]
        [TestCase("toys \"R\" Us", new[] { "gift" })]
        [TestCase("toys and groceries", new[] { "gift", "food", "groceries" })]
        public void Test_Expenses_Models_Tagging_AddTagsToItems_OneItem(string itemDescription, string[] expectedTagNames)
        {
            var danCardNumber = FactoryData.Accounts.Dan.Cards.DanCard.CardNumber;
            var danCard = Card.FindByCardNumber(danCardNumber);
            var statement = danCard.Statements.First();

            var itemId = Guid.NewGuid();
            var item = new Item(itemId, statement.Id, "1", itemDescription, DateTime.Now, 100.40M);
            using (var db = new Context())
            {
                db.Items.Add(item);
                db.SaveChanges();
            }

            TaggingModel.AddTagsToItems(Taggings, new[] { item.Id });

            var reloadedItem = Item.Get(itemId);
            CollectionAssert.AreEquivalent(expectedTagNames, reloadedItem.Tags.Select(t => t.Name));
        }

        public static IEnumerable MultipleItems()
        {
            yield return new TestCaseData(
                new List<string>
                {
                    "toys \"R\" Us",
                    "ToYs \"R\" Us",
                    "toy \"R\" Us",
                    "meridian and coffee"
                },
                new List<string[]>
                {
                    new[] { "gift" },
                    new[] { "gift" },
                    new string[] { },
                    new[] { "coffee" }
                }
            );
        }

        [TestCaseSource(nameof(MultipleItems))]
        public void Test_Expenses_Models_Tagging_AddTagsToItems_MultipleItems(List<string> itemDescriptions, List<string[]> expectedTagNamesList)
        {
            var danCardNumber = FactoryData.Accounts.Dan.Cards.DanCard.CardNumber;
            var danCard = Card.FindByCardNumber(danCardNumber);
            var statement = danCard.Statements.First();

            var i = 0;
            var newItems = new List<Item>(itemDescriptions.Count());
            foreach (var desc in itemDescriptions)
            {
                i += 1;

                var item = new Item(Guid.NewGuid(), statement.Id, i.ToString(), desc, DateTime.Now, 100.40M);
                newItems.Add(item);
            }

            using (var db = new Context())
            {
                db.Items.AddRange(newItems);
                db.SaveChanges();
            }

            TaggingModel.AddTagsToItems(Taggings, newItems.Select(item => item.Id));
            var reloadedItems = newItems
                .Select(item => item.Id)
                .Select(Item.Get);

            var j = 0;
            foreach (var item in reloadedItems)
            {
                Assert.That(
                    item.Tags.Select(tag => tag.Name),
                    Is.EquivalentTo(expectedTagNamesList[j])
                );

                j += 1;
            }
        }
    }
}
