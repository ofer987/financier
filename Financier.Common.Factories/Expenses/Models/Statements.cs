using System;
using System.Collections.Generic;
using Financier.Common.Expenses.Models;

namespace Financier.Common
{
    public partial class Factories
    {
        public static Statement NewStatement(Card card, DateTime postedAt)
        {
            var utcAt = DateTime.SpecifyKind(postedAt, DateTimeKind.Utc);

            return new Statement
            {
                Id = Guid.NewGuid(),
                CardId = card.Id,
                PostedAt = postedAt,
                Items = new List<Item>()
            };
        }

        public static Statement CreateSimpleStatement(Card card, DateTime postedAt)
        {
            var statement = NewStatement(card, postedAt);

            using (var db = new Context())
            {
                db.Statements.Add(statement);
                db.SaveChanges();
            }

            return statement;
        }
    }
}

