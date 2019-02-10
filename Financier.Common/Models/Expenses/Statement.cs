using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Financier.Common.Models.Expenses
{
    public class Statement
    {
        [Key]
        [Required]
        public Guid Id { get; set; }

        public Card Card { get; set; }

        public Guid CardId { get; set; }

        [Required]
        public DateTime PostedAt { get; set; }

        public List<Item> Items { get; set; }

        public int Year => PostedAt.Year;

        public int Month => PostedAt.Month;

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var other = obj as Statement;
            if (other == null)
            {
                return false;
            }

            if (PostedAt != other.PostedAt)
            {
                return false;
            }

            if ((Items ?? new List<Item>()).Count != (other.Items ?? new List<Item>()).Count)
            {
                return false;
            }

            foreach (var xItem in Items)
            {
                if (!other.Items.Any(yItem => yItem == xItem))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool operator ==(Statement x, Statement y)
        {
            if (object.ReferenceEquals(x, null))
            {
                return (object.ReferenceEquals(y, null));
            }

            return x.Equals(y);
        }

        public static bool operator !=(Statement x, Statement y)
        {
            return !(x == y);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"{nameof(Id)}: ({Id})");
            sb.AppendLine($"{nameof(PostedAt)}: ({PostedAt})");
            sb.AppendLine($"Items:");
            foreach (var item in Items ?? new List<Item>())
            {
                sb.AppendLine($"\t{item}");
            }

            return sb.ToString();
        }
    }
}
