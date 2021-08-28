using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

using ItemTagger = Financier.Common.Expenses.ItemTagger;
using Financier.Common.Expenses.Models;

namespace Financier.Common.Tests.Expenses
{
    public class ItemTaggerTests : InitializedDatabaseTests
    {
        public static IEnumerable<ItemTagger> Rules;

        [SetUp]
        public void Setup()
        {
            var foodRules1 = new ItemTagger("freshco", new[] { "food", "groceries" });
            var foodRules2 = new ItemTagger("groceries", new[] { "food", "groceries" });

            var coffeeRules = new ItemTagger("meridian", new[] { "coffee" });
            var giftRules = new ItemTagger("toys", new[] { "gift" });

            Rules = new List<ItemTagger>
            {
                foodRules1,
                foodRules2,
                coffeeRules,
                giftRules
            };
        }

        [TestCase("FreshCo Bathurst and Steeles", new[] { "food", "groceries" })]
        [TestCase("Bathurst and Steeles freshco", new[] { "food", "groceries" })]
        [TestCase("groceries bodega", new[] { "food", "groceries" })]
        [TestCase("meridian i. i. c place", new[] { "coffee" })]
        [TestCase("toys \"R\" Us", new[] { "gift" })]
        [TestCase("toys and groceries", new[] { "gift", "food", "groceries" })]
        public void Test_Expenses_Models_ItemTaggerTests_AddTagsToItems_OneItem(string itemDescription, string[] expectedTagNames)
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

            ItemTagger.AddTagsToItems(Rules, new[] { item.Id });

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
        public void Test_Expenses_Models_ItemTaggerTests_AddTagsToItems_MultipleItems(List<string> itemDescriptions, List<string[]> expectedTagNamesList)
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

            ItemTagger.AddTagsToItems(Rules, newItems.Select(item => item.Id));
            var reloadedItems = newItems
                .Select(item => item.Id)
                .Select(Item.Get);

            var j = 0;
            foreach (var item in reloadedItems)
            {
                Assert.That(
                    (item.Tags ?? Enumerable.Empty<Tag>()).Select(tag => tag.Name),
                    Is.EquivalentTo(expectedTagNamesList[j])
                );

                j += 1;
            }
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
        public void Test_Expenses_Models_ItemTaggerTests_Single_Regex_IsMatch(
            string itemDescription,
            string regex,
            bool expected
        )
        {
            var tagging = new ItemTagger(regex, new string[] { });

            Assert.That(tagging.IsMatch(itemDescription), Is.EqualTo(expected));
        }

        [TestCase(
            FactoryData.Accounts.Dan.Cards.DanCard.Statements.June.Items.Porsche912.Description,
            new string[] {
                @"^welcome",
                @"porsche \d{3}",
                @"thank you"
            },
            true
        )]
        [TestCase(
            FactoryData.Accounts.Dan.Cards.DanCard.Statements.June.Items.Porsche912.Description,
            new string[] {
                @"^dollars$",
                @"ferrari"
            },
            false
        )]
        public void Test_Expenses_Models_ItemTaggerTests_Multiple_Regexes_IsMatch(
            string itemDescription,
            string[] regexes,
            bool expected
        )
        {
            var tagging = new ItemTagger(regexes, new string[] { });

            Assert.That(tagging.IsMatch(itemDescription), Is.EqualTo(expected));
        }

        [TestCase("my_house", new[] { FactoryData.Tags.Dog.Name, FactoryData.Tags.Coffee.Name })]
        public void Test_Expenses_Models_ItemTaggerTests_AddTagsMultipleTimesToOneItem(string itemDescription, string[] expectedTagNames)
        {
            var taggings = expectedTagNames
                .Select(t => new ItemTagger(string.Empty, new[] { t }))
                .ToList();

            var danCardNumber = FactoryData.Accounts.Dan.Cards.DanCard.CardNumber;
            var danCard = Card.FindByCardNumber(danCardNumber);
            var statement = danCard.Statements.First();

            var newItemId = Guid.NewGuid();
            var newItem = new Item(newItemId, statement.Id, "1", itemDescription, DateTime.Now, 100.40M);

            using (var db = new Context())
            {
                db.Items.Add(newItem);
                db.SaveChanges();
            }

            foreach (var tagging in taggings)
            {
                tagging.AddTags(newItem.Id);
            }

            var reloadedItem = Item.Get(newItemId);

            reloadedItem.Tags.Select(t => t.Name);
            CollectionAssert.AreEquivalent(expectedTagNames.Select(t => t.ToLower()), reloadedItem.Tags.Select(t => t.Name));
        }
    }
}
