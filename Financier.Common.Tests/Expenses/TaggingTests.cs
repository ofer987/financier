using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

using Financier.Common.Expenses;
using Financier.Common.Expenses.Models;

namespace Financier.Common.Tests.Expenses
{
    public class TaggingTests : DatabaseFixture
    {
        [TestCase(
            FactoryData.Accounts.Dan.Cards.DanCard.Statements.June.Items.Porsche.ItemId,
            new[] { "fun", "fast" },
            new[] { "fun", "fast" }
        )]
        [TestCase(
            FactoryData.Accounts.Dan.Cards.DanCard.Statements.June.Items.Porsche.ItemId,
            new[] { "fun", "fast" },
            new[] { "fast", "fun" }
        )]
        [TestCase(
            FactoryData.Accounts.Dan.Cards.DanCard.Statements.June.Items.Porsche.ItemId,
            new string[] { },
            new[] { "fast", "fun" }
        )]
        [TestCase(
            FactoryData.Accounts.Dan.Cards.DanCard.Statements.June.Items.Porsche.ItemId,
            new[] { "my-dream" },
            new[] { "fast", "fun", "my-dream" }
        )]
        public void Test_Expenses_Models_Tagging_AddTags(
            string itemNumber,
            string[] newTagNames,
            string[] expectedTagNames
        )
        {
            var item = Item.GetByItemId(itemNumber);
            var tagging = new Tagging(string.Empty, newTagNames);

            var actualTags = tagging.AddTags(item.Id);

            Assert.That(
                actualTags.Select(tag => tag.Name),
                Is.EquivalentTo(expectedTagNames)
            );

            var reloadedItem = Item.GetByItemId(itemNumber);
            Assert.That(
                reloadedItem.Tags.Select(tag => tag.Name),
                Is.EquivalentTo(expectedTagNames)
            );
        }

        [TestCase(
            FactoryData.Accounts.Dan.Cards.DanCard.Statements.June.Items.Porsche.Description,
            "Porsche",
            true
        )]
        [TestCase(
            FactoryData.Accounts.Dan.Cards.DanCard.Statements.June.Items.Porsche.Description,
            "porsche",
            true
        )]
        [TestCase(
            FactoryData.Accounts.Dan.Cards.DanCard.Statements.June.Items.Porsche.Description,
            "^porsche$",
            false
        )]
        public void Test_Expenses_Models_Tagging_IsMatch(
            string itemDescription,
            string regex,
            bool expected
        )
        {
            var tagging = new Tagging(regex, new string[] { });

            Assert.That(tagging.IsMatch(itemDescription), Is.EqualTo(expected));
        }

        public static List<Tagging> Taggings = new List<Tagging>();

        [SetUp]
        public void Setup()
        {
            var foodTaggings1 = new Tagging("freshco", new[] { "food", "groceries" });
            var foodTaggings2 = new Tagging("groceries", new[] { "food", "groceries" });

            var coffeeTaggings = new Tagging("meridian", new[] { "coffee" });
            var giftTaggings = new Tagging("toys", new[] { "gift" });

            Taggings = new List<Tagging>
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
        [TestCase("toys \"R\" Us", new[] { "toys" })]
        [TestCase("toys and groceries", new[] { "toys", "food", "groceries" })]
        public void Test_Expenses_Models_Tagging_AddTagsToItems_OneItem(string itemDescription, string[] expectedTagNames)
        {
            var danCardNumber = FactoryData.Accounts.Dan.Cards.DanCard.CardNumber;
            var danCard = Card.FindByCardNumber(danCardNumber);
            var statement = danCard.Statements.First();

            var item = new Item(Guid.NewGuid(), statement.Id, "1", itemDescription, DateTime.Now, 100.40M);
            using (var db = new Context())
            {
                db.Items.Add(item);
                db.SaveChanges();
            }

            Console.WriteLine(Taggings.Count);
            Tagging.AddTagsToItems(Taggings, new[] { item.Id });
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

            Tagging.AddTagsToItems(Taggings, newItems.Select(item => item.Id));
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
