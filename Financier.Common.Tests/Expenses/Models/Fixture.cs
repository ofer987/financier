using System;
using System.Linq;
using System.Collections.Generic;

using Financier.Common.Expenses.Models;

namespace Financier.Common.Tests.Expenses.Models
{
    public class MyFactories : Factories
    {
        public static Guid DanCardId = Guid.NewGuid();
        public const string DanCardNumber = "1234567";
        public static Func<Card> GetDanCard = () => new Card
        {
            Id = DanCardId,
            Number = DanCardNumber,
            Statements = new List<Statement>()
        };

        public static Guid RonCardId = Guid.NewGuid();
        public const string RonCardNumber = "34875487543";
        public static Func<Card> GetRonCard = () => new Card
        {
            Id = RonCardId,
            Number = RonCardNumber,
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
    }

    public abstract class Fixture : DatabaseAbstractFixture
    {
        protected override void InitDb()
        {
            using (var db = new Context())
            {
                var funTag = MyFactories.GetFunTag();
                var fastTag = MyFactories.GetFastTag();
                db.Tags.Add(funTag);
                db.Tags.Add(fastTag);

                var danCard = MyFactories.GetDanCard();
                var ronCard = MyFactories.GetRonCard();
                db.Cards.Add(danCard);
                db.Cards.Add(ronCard);
                db.SaveChanges();

                var juneStatement = MyFactories.GetJuneStatement();
                var julyStatement = MyFactories.GetJulyStatement();
                var ronsCrazyStatement = MyFactories.GetRonsCrazyStatement();
                juneStatement.Items.Add(MyFactories.GetPorscheItem(new[] { funTag, fastTag }));
                juneStatement.Items.Add(MyFactories.GetFerrariItem(new[] { funTag }));
                ronsCrazyStatement.Items.Add(MyFactories.GetLamboItem(new[] { fastTag }));

                db.Statements.Add(juneStatement);
                db.Statements.Add(julyStatement);
                db.Statements.Add(ronsCrazyStatement);
                db.SaveChanges();
            }
        }
    }
}
