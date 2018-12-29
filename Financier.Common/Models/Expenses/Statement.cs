using System;
using System.Collections.Generic;

namespace Financier.Common.Models.Expenses
{
    public class Statement
    {
        public Guid Id { get; set; }

        public Card Card { get; set; }

        public Guid CardId { get; set; }

        public DateTime PostedAt { get; set; }

        public ICollection<Item> Items { get; set; }

        public int Year => PostedAt.Year;

        public int Month => PostedAt.Month;
    }
}
