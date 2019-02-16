using System;
using System.Collections.Generic;

using Financier.Common.Models.Expenses;

namespace Financier.Tests.Fixtures
{
    public static class Cards
    {
        public static Card SimpleCard => new Card
        {
            Id = Guid.NewGuid(),
            Number = "1234",
            Statements = new List<Statement>()
        };
    }
}

