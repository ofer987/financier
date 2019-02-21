using System;
using System.Collections.Generic;
using Financier.Common.Expenses.Models;

namespace Financier.Tests.Expenses
{
    public partial class Fixtures
    {
        public static Statement GetSimpleStatement(Card card)
        {
            return new Statement 
            {
                Id = Guid.NewGuid(),
                CardId = card.Id,
                PostedAt = new DateTime(2019, 1, 1),
                Items = new List<Item>()
            };
        }
    }
}

