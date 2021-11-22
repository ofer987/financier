using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Financier.Common.Expenses.Models
{
    [Table("Expenses_Accounts")]
    public class Account : IAccount
    {
        [Key]
        [Required]
        // TODO: make unique
        public string Name { get; set; }

        public List<Card> Cards { get; set; } = new List<Card>();

        // TODO: write tests
        public static Account FindByName(string name)
        {
            using (var db = new Context())
            {
                return db.Accounts
                    .First(account => account.Name == name);
            }
        }

        // TODO: write tests
        public IEnumerable<Item> GetAllItems(DateTime from, DateTime to)
        {
            using (var db = new Context())
            {
                return db.Items
                    .Include(item => item.ItemTags)
                        .ThenInclude(it => it.Tag)
                    .Where(item => item.PostedAt >= from)
                    .Where(item => item.PostedAt < to)
                    .ToArray();
            }
        }

        public void Delete()
        {
            using (var db = new Context())
            {
                db.Accounts.Remove(this);
                db.SaveChanges();
            }
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var other = obj as Account;
            if (other == null)
            {
                return false;
            }

            if ((Name ?? string.Empty) != (other.Name ?? string.Empty))
            {
                return false;
            }

            if (Cards.Count != other.Cards.Count)
            {
                return false;
            }

            foreach (var card in Cards)
            {
                if (!other.Cards.Any(yState => yState == card))
                {
                    return false;
                }
            }

            return true;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"{nameof(Name)}: ({Name})");
            sb.AppendLine($"Cards:");
            foreach (var card in Cards ?? new List<Card>())
            {
                sb.AppendLine($"\t{card}");
            }

            return sb.ToString();
        }

        public static bool operator ==(Account x, Account y)
        {
            if (object.ReferenceEquals(x, null))
            {
                return (object.ReferenceEquals(y, null));
            }

            return x.Equals(y);
        }

        public static bool operator !=(Account x, Account y)
        {
            return !(x == y);
        }
    }
}
