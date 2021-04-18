using System;
using System.Collections.Generic;
using Financier.Common.Expenses.Models;

namespace Financier.Common
{
    public partial class Factories
    {
        public static Statement CreateSimpleStatement(Card card, DateTime postedAt)
        {
            return new Statement 
            {
                Id = Guid.NewGuid(),
                CardId = card.Id,
                PostedAt = postedAt,
                Items = new List<Item>()
            };
        }
    }
}

