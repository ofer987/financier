using System;
using System.Collections.Generic;

namespace Financier.Common.Models.Expenses
{
    public class Card
    {
        public Guid Id { get; set; }

        public string RealId { get; set; }

        public ICollection<Statement> Statements { get; set; }
    }
}
