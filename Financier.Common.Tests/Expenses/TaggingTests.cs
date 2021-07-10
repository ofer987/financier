using System;
using System.Linq;
using NUnit.Framework;

using TaggingModel = Financier.Common.Expenses.Tagging;
using Financier.Common.Expenses.Models;

namespace Financier.Common.Tests.Expenses
{
    public class TaggingTests : InitializedDatabaseTests
    {
        [SetUp]
        public void Setup()
        {
            var foodTaggings1 = new TaggingModel("freshco", new[] { "food", "groceries" });
            var foodTaggings2 = new TaggingModel("groceries", new[] { "food", "groceries" });

            var coffeeTaggings = new TaggingModel("meridian", new[] { "coffee" });
            var giftTaggings = new TaggingModel("toys", new[] { "gift" });
        }

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
            var tagging = new TaggingModel(string.Empty, newTagNames);

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
        public void Test_Expenses_Models_Single_Regex_Tagging_IsMatch(
            string itemDescription,
            string regex,
            bool expected
        )
        {
            var tagging = new TaggingModel(regex, new string[] { });

            Assert.That(tagging.IsMatch(itemDescription), Is.EqualTo(expected));
        }

        [TestCase(
            new string[] {
                FactoryData.Accounts.Dan.Cards.DanCard.Statements.June.Items.Porsche912.Description,
            },
            new string[] {
                @"^welcome",
                @"porsche \d{3}",
                @"thank you"
            },
            true
        )]
        [TestCase(
            new string[] {
                FactoryData.Accounts.Dan.Cards.DanCard.Statements.June.Items.Porsche912.Description,
            },
            new string[] {
                @"^dollars$",
                @"ferrari"
            },
            false
        )]
        public void Test_Expenses_Models_Multiple_Regexes_Tagging_IsMatch(
            string itemDescription,
            string[] regexes,
            bool expected
        )
        {
            var tagging = new TaggingModel(regexes, new string[] { });

            Assert.That(tagging.IsMatch(itemDescription), Is.EqualTo(expected));
        }

        [TestCase("my_house", new[] { FactoryData.Tags.Dog.Name, FactoryData.Tags.Coffee.Name })]
        public void Test_Expenses_Models_Tagging_AddTagsMultipleTimesToOneItem(string itemDescription, string[] expectedTagNames)
        {
            var taggings = expectedTagNames
                .Select(t => new TaggingModel(string.Empty, new[] { t }))
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
