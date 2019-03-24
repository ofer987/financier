using System;
using System.Linq;
using System.Collections.Generic;

using Financier.Common.Expenses.Models;

namespace Financier.Common.Tests.Expenses.Models
{
    public class MyFactories : Factories
    {
        public static class SavingsCard
        {
            public static Func<Card> GetCard = () => new Card
            {
                Id = Guid.NewGuid(),
                Number = "123485753492",
                CardType = CardTypes.Bank,
                Statements = new List<Statement>()
            };

            public static class June
            {
                public static Func<Statement> GetStatement = () => new Statement
                {
                    Id = Guid.NewGuid(),
                       PostedAt = new DateTime(2019, 6, 31)
                };

                public static class Items
                {
                    public static Func<IEnumerable<Tag>, Item> GetGroceries = (tags) => new Item
                    {
                        Id = Guid.NewGuid(),
                        Amount = 104.50M,
                        Description = "Fresco",
                        ItemId = Guid.NewGuid().ToString(),
                        PostedAt = new DateTime(2019, 6, 5),
                        TransactedAt = new DateTime(2019, 6, 5),
                        ItemTags = tags.Select(tag => new ItemTag { Tag = tag }).ToList(),
                    };

                    public static Func<IEnumerable<Tag>, Item> GetCoffee = (tags) => new Item
                    {
                        Id = Guid.NewGuid(),
                        Amount = 4.20M,
                        Description = "IQ",
                        ItemId = Guid.NewGuid().ToString(),
                        ItemTags = tags.Select(tag => new ItemTag { Tag = tag }).ToList(),
                        PostedAt = new DateTime(2019, 6, 15),
                        TransactedAt = new DateTime(2019, 6, 15)
                    };

                    public static Func<IEnumerable<Tag>, Item> GetCreditCardPayment = (tags) => new Item
                    {
                        Id = Guid.NewGuid(),
                        Amount = -967.00M,
                        Description = "Transfer to Credit Card",
                        ItemId = Guid.NewGuid().ToString(),
                        ItemTags = tags.Select(tag => new ItemTag { Tag = tag }).ToList(),
                        PostedAt = new DateTime(2019, 6, 22),
                        TransactedAt = new DateTime(2019, 6, 22)
                    };
                }
            }

            public static class July
            {
                public static Func<Statement> GetStatement = () => new Statement
                {
                    Id = Guid.NewGuid(),
                    PostedAt = new DateTime(2019, 7, 31)
                };

                public static class Items
                {
                    public static Func<IEnumerable<Tag>, Item> GetGroceries = (tags) => new Item
                    {
                        Id = Guid.NewGuid(),
                           Amount = 98.25M,
                           Description = "Your Community Grocer",
                           ItemId = Guid.NewGuid().ToString(),
                           ItemTags = tags.Select(tag => new ItemTag { Tag = tag }).ToList(),
                           PostedAt = new DateTime(2019, 7, 8),
                           TransactedAt = new DateTime(2019, 7, 7)
                    };

                    public static Func<IEnumerable<Tag>, Item> GetCoffee = (tags) => new Item
                    {
                        Id = Guid.NewGuid(),
                           Amount = 4.20M,
                           Description = "IQ",
                           ItemId = Guid.NewGuid().ToString(),
                           ItemTags = tags.Select(tag => new ItemTag { Tag = tag }).ToList(),
                           PostedAt = new DateTime(2019, 7, 14),
                           TransactedAt = new DateTime(2019, 7, 14)
                    };

                    public static Func<IEnumerable<Tag>, Item> GetCreditCardPayment = (tags) => new Item
                    {
                        Id = Guid.NewGuid(),
                           Amount = -678.00M,
                           Description = "Transfer to Credit Card",
                           ItemId = Guid.NewGuid().ToString(),
                           ItemTags = tags.Select(tag => new ItemTag { Tag = tag }).ToList(),
                           PostedAt = new DateTime(2019, 7, 22),
                           TransactedAt = new DateTime(2019, 7, 22)
                    };
                }
            }
        }

        public static class RonCard
        {
            public static Guid RonCardId = Guid.NewGuid();
            public const string RonCardNumber = "34875487543";
            public static Func<Card> GetRonCard = () => new Card
            {
                Id = RonCardId,
                   Number = RonCardNumber,
                   CardType = CardTypes.Credit,
                   Statements = new List<Statement>()
            };

            public static Guid RonsCrazyStatementId = Guid.NewGuid();
            public static Func<Statement> GetRonsCrazyStatement = () => new Statement
            {
                Id = RonsCrazyStatementId,
                   CardId = RonCardId,
                   Items = new List<Item>(),
                   PostedAt = new DateTime(2025, 7, 1)
            };
        }

        public static class DanCard
        {
            public static Guid DanCardId = Guid.NewGuid();
            public const string DanCardNumber = "1234567";
            public static Func<Card> GetDanCard = () => new Card
            {
                Id = DanCardId,
                   Number = DanCardNumber,
                   CardType = CardTypes.Credit,
                   Statements = new List<Statement>()
            };

            public static Guid JuneStatementId = Guid.NewGuid();
            public static Func<Statement> GetJuneStatement = () => new Statement
            {
                Id = JuneStatementId,
                   CardId = DanCardId,
                   Items = new List<Item>(),
                   PostedAt = new DateTime(2025, 7, 1)
            };

            public static Guid JulyStatementId = Guid.NewGuid();
            public static Func<Statement> GetJulyStatement = () => new Statement
            {
                Id = JulyStatementId,
                   CardId = DanCardId,
                   Items = new List<Item>(),
                   PostedAt = new DateTime(2025, 8, 1)
            };
        }

        public const string PorscheItemId = "1234";
        public static Guid PorscheId = Guid.NewGuid();
        public static Func<IEnumerable<Tag>, Item> GetPorscheItem = (tags) => new Item
        {
            Id = PorscheId,
            Amount = 300000.00M,
            Description = "Porsche 911",
            ItemId = PorscheItemId,
            ItemTags = tags.Select(tag => new ItemTag { Tag = tag }).ToList(),
            PostedAt = new DateTime(2025, 6, 5),
            TransactedAt = new DateTime(2025, 6, 5),
        };

        public const string LamboItemId = "9481";
        public static Guid LamboId = Guid.NewGuid();
        public static Func<IEnumerable<Tag>, Item> GetLamboItem = (tags) => new Item
        {
            Id = LamboId,
            Amount = 300000.00M,
            Description = "Lambo",
            ItemId = LamboItemId,
            ItemTags = tags.Select(tag => new ItemTag { Tag = tag }).ToList(),
            PostedAt = new DateTime(2025, 6, 5),
            TransactedAt = new DateTime(2025, 6, 5),
        };

        public const string FerrariItemId = "458487";
        public static Guid FerrariId = Guid.NewGuid();
        public static Func<IEnumerable<Tag>, Item> GetFerrariItem = (tags) => new Item
        {
            Id = FerrariId,
            Amount = 300000.00M,
            Description = "Ferrari",
            ItemId = FerrariItemId,
            ItemTags = tags.Select(tag => new ItemTag { Tag = tag }).ToList(),
            PostedAt = new DateTime(2025, 6, 5),
            TransactedAt = new DateTime(2025, 6, 5),
        };

        public static class Tags
        {
            public static Func<Tag> GetFunTag = () => new Tag
            {
                Id = Guid.NewGuid(),
                   Name = "Fun"
            };

            public static Func<Tag> GetFastTag = () => new Tag
            {
                Id = Guid.NewGuid(),
                   Name = "Fast"
            };

            public static Func<Tag> GetDogTag = () => new Tag
            {
                Id = Guid.NewGuid(),
                   Name = "like-a-dog"
            };

            public static Func<Tag> GetCreditCardPayment = () => new Tag
            {
                Id = Guid.NewGuid(),
                Name = "credit-card-payment"
            };
        }
    }

    public abstract class Fixture : DatabaseAbstractFixture
    {
        protected override void InitDb()
        {
            using (var db = new Context())
            {
                var funTag = MyFactories.Tags.GetFunTag();
                var fastTag = MyFactories.Tags.GetFastTag();
                var dogTag = MyFactories.Tags.GetDogTag();
                db.Tags.Add(funTag);
                db.Tags.Add(fastTag);

                var danCard = MyFactories.DanCard.GetDanCard();
                var ronCard = MyFactories.RonCard.GetRonCard();
                db.Cards.Add(danCard);
                db.Cards.Add(ronCard);
                db.SaveChanges();

                var juneStatement = MyFactories.DanCard.GetJuneStatement();
                var julyStatement = MyFactories.DanCard.GetJulyStatement();
                var ronsCrazyStatement = MyFactories.RonCard.GetRonsCrazyStatement();
                juneStatement.Items.Add(MyFactories.GetPorscheItem(new[] { funTag, fastTag }));
                juneStatement.Items.Add(MyFactories.GetFerrariItem(new[] { funTag }));
                ronsCrazyStatement.Items.Add(MyFactories.GetLamboItem(new[] { fastTag, dogTag }));

                db.Statements.Add(juneStatement);
                db.Statements.Add(julyStatement);
                db.Statements.Add(ronsCrazyStatement);
                db.SaveChanges();
            }
        }
    }
}
