using System;
using System.Collections.Generic;

using Financier.Common.Expenses.Models;

namespace Financier.Common
{
    public partial class Factories
    {
        public static Card NewCard(Account account, string number)
        {
            return new Card
            {
                AccountName = account.Name,
                Id = Guid.NewGuid(),
                Number = number,
                Statements = new List<Statement>()
            };

        }

        public static Card CreateCard(Account account, string number)
        {
            var card = NewCard(account, number);

            using (var db = new Context())
            {
                db.Cards.Add(card);
                db.SaveChanges();
            }

            return card;
        }

        public static Card CreateBankCard(Account account, string number)
        {
            var card = new Card
            {
                AccountName = account.Name,
                Id = Guid.NewGuid(),
                Number = number,
                CardType = CardTypes.Bank,
                Statements = new List<Statement>()
            };

            using (var db = new Context())
            {
                db.Cards.Add(card);
                db.SaveChanges();
            }

            return card;
        }

        public static Card CreateCreditCard(Account account, string number)
        {
            var card = new Card
            {
                AccountName = account.Name,
                Id = Guid.NewGuid(),
                Number = number,
                CardType = CardTypes.Credit,
                Statements = new List<Statement>()
            };

            using (var db = new Context())
            {
                db.Cards.Add(card);
                db.SaveChanges();
            }

            return card;
        }
    }
}

