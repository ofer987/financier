using System;
using System.Collections.Generic;

using Financier.Common.Expenses.Models;

namespace Financier.Tests.Expenses
{
    public partial class Fixtures
    {
        public static Card SimpleCard => new Card
        {
            Id = Guid.NewGuid(),
            Number = "1234",
            Statements = new List<Statement>()
        };
    }
}

