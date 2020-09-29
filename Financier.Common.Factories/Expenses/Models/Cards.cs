using System;
using System.Collections.Generic;

using Financier.Common.Expenses.Models;

namespace Financier.Common
{
    public partial class Factories
    {
        public static Func<Account, Card> SimpleCard = (account) => new Card
        {
            Owner = account,
            Id = Guid.NewGuid(),
            Number = "1234",
            Statements = new List<Statement>()
        };
    }
}

