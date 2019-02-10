using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Financier.Common.Models.Expenses
{
    public class Card
    {
        [Key]
        [Required]
        public Guid Id { get; set; }

        // [Key]
        [Required]
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
                if (!other.Statements.Any(yState => yState == statement))
                {
                    return false;
                }
            }

            return true;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"{nameof(Id)}: ({Id})");
            sb.AppendLine($"{nameof(Number)}: ({Number ?? string.Empty})");
            sb.AppendLine($"Statements:");
            foreach (var statement in Statements ?? new List<Statement>())
            {
                sb.AppendLine($"\t{statement}");
            }

            return sb.ToString();
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
