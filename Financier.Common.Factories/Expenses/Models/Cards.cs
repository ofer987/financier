using System;
using System.Collections.Generic;

using Financier.Common.Expenses.Models;

namespace Financier.Common
{
    public partial class Factories
    {
        public static Func<Account, string, Card> CreateCard = (account, number) => new Card
        {
            Owner = account,
            Id = Guid.NewGuid(),
            Number = number,
            Statements = new List<Statement>()
        };

        public static Func<Account, string, Card> CreateBankCard = (account, number) => new Card
        {
            Owner = account,
            Id = Guid.NewGuid(),
            Number = number,
            CardType = CardTypes.Bank,
            Statements = new List<Statement>()
        };

        public static Func<Account, string, Card> CreateCreditCard = (account, number) => new Card
        {
            Owner = account,
            Id = Guid.NewGuid(),
            Number = number,
            CardType = CardTypes.Credit,
            Statements = new List<Statement>()
        };
    }
}

