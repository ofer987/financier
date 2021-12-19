using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

using Financier.Common.Extensions;

namespace Financier.Common.Expenses.Models
{
    public abstract class StatementRecord
    {
        public virtual string AccountName { get; set; }
        public virtual string ItemId { get; set; }
        public virtual string Number { get; set; }

        public abstract CardTypes CardType { get; }

        private Card _card = null;
        public Card GetCard()
        {
            if (_card is not null)
            {
                return _card;
            }

            return (_card = FindOrCreateCard());
        }

        private Statement _statement = null;
        public Statement GetStatement(DateTime postedAt)
        {
            if (_statement is not null)
            {
                return _statement;
            }

            return (_statement = FindOrCreateStatement(GetCard().Id, postedAt));
        }

        public Statement FindOrCreateStatement(Guid cardId, DateTime postedAt)
        {
            using (var db = new Context())
            {
                var statement = db.Statements
                    .Include(stmt => stmt.Card)
                    .Include(stmt => stmt.Items)
                    .Where(stmt => stmt.CardId == cardId)
                    .Where(stmt => stmt.PostedAt == postedAt)
                    .FirstOrDefault();

                if (statement is null)
                {
                    statement = new Statement
                    {
                        Id = Guid.NewGuid(),
                        PostedAt = postedAt,
                        CardId = cardId,
                        Items = new List<Item>(),
                    };
                    db.Statements.Add(statement);
                    db.SaveChanges();

                    return statement;
                }

                return statement;
            }
        }

        public Card FindOrCreateCard()
        {
            // TODO: move the getter
            var cardNumber = CleanNumber(Number);
            using (var db = new Context())
            {
                var card = db.Cards
                    .Include(cd => cd.Statements)
                        .ThenInclude(stmt => stmt.Items)
                    .FirstOrDefault(cd => cd.Number == cardNumber);

                if (card is null)
                {
                    var newCard = new Card
                    {
                        Id = Guid.NewGuid(),
                        Number = cardNumber,
                        CardType = CardType,
                        Statements = new List<Statement>(),
                        AccountName = AccountName
                    };
                    db.Cards.Add(newCard);
                    db.SaveChanges();

                    // Reload the card
                    card = db.Cards
                        .Include(cd => cd.Statements)
                        .ThenInclude(stmt => stmt.Items)
                        .FirstOrDefault(cd => cd.Number == cardNumber);
                }

                return card;
            }
        }

        public string CleanNumber(string val)
        {
            var regex = new Regex(@"^\s*'?\s*(\w+)\s*'?\s*$");

            if (!regex.Match(val).Success || regex.Match(val).Groups.Count < 2)
            {
                throw new Exception($"Could not get card number from ({val})");
            }

            return regex.Match(val).Groups[1].Value;
        }

        public virtual void Validate()
        {
            if (ItemId.Trim().IsNullOrEmpty())
            {
                throw new ArgumentException("cannot be blank or whitespace", nameof(ItemId));
            }
        }

        protected DateTime ToDateTime(string str)
        {
            return DateTime.ParseExact(str, "yyyyMMdd", null);
        }

        public abstract Item CreateItem(Guid statementId);
    }
}
