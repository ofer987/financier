using System;
using System.Collections.Generic;
using System.Linq;

namespace Financier.Common.Models.Expenses
{
    public class Card
    {
        public Guid Id { get; set; }

        public string Number { get; set; }

        public List<Statement> Statements { get; set; }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var other = obj as Card;
            if (other == null)
            {
                return false;
            }

            if ((Number ?? string.Empty) != (other.Number ?? string.Empty))
            {
                return false;
            }

            if (Statements.Count != other.Statements.Count)
            {
                return false;
            }

            foreach (var statement in Statements)
            {
                if (!other.Statements.Any(yState => yState.Id == statement.Id))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool operator ==(Card x, Card y)
        {
            if (object.ReferenceEquals(x, null))
            {
                return (object.ReferenceEquals(y, null));
            }

            return x.Equals(y);
        }

        public static bool operator !=(Card x, Card y)
        {
            return !(x == y);
        }
    }
}
