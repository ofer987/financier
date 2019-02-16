using System;
using System.Collections.Generic;
using Financier.Common.Models.Expenses;

namespace Financier.Tests.Fixtures
{
    public static class Statements
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

