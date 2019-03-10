using System;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

using Financier.Common.Extensions;

namespace Financier.Common.Expenses.Models
{
    [Table("Expenses_Items")]
    public class Item
    {
        public static Item[] GetAll()
        {
            using (var db = new Context())
            {
                return db.Items
                    .Include(item => item.ItemTags)
                    .Reject(item => item.ItemTags.Any())
                    .ToArray();
            }
        }

        [Key]
        [Required]
        public Guid Id { get; set; }

        public Guid StatementId { get; set; }

        [Required]
        public string ItemId { get; set; }

        public Statement Statement { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public DateTime TransactedAt { get; set; }

        [Required]
        public DateTime PostedAt { get; set; }

        public List<ItemTag> ItemTags { get; set; } = new List<ItemTag>();

        public IEnumerable<Tag> Tags => ItemTags.Select(it => it.Tag);

        public Item(string description, DateTime transactedAt, DateTime postedAt, decimal amount)
        {
            Description = description;
            TransactedAt = transactedAt;
            PostedAt = postedAt;
            Amount = amount;
        }

        public Item()
        {
        }

        public void Delete()
        {
            using (var db = new Context())
            {
                foreach (var itemTag in ItemTags)
                {
                    itemTag.Delete();
                }
                db.Items.Remove(this);
                db.SaveChanges();
            }
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var other = obj as Item;
            if (other == null)
            {
                return false;
            }

            if ((Description ?? string.Empty) != (other.Description ?? string.Empty)
                    || Amount != other.Amount 
                    || TransactedAt != other.TransactedAt 
                    || PostedAt != other.PostedAt)
            {
                return false;
            }

            return true;
        }

        public static bool operator ==(Item x, Item y)
        {
            if (object.ReferenceEquals(x, null))
            {
                return (object.ReferenceEquals(y, null));
            }

            return x.Equals(y);
        }

        public static bool operator !=(Item x, Item y)
        {
            return !(x == y);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"{nameof(Id)}: ({Id})");
            sb.AppendLine($"{nameof(Description)}: ({Description ?? string.Empty})");
            sb.AppendLine($"{nameof(Amount)}: ({AmountString()})");
            sb.AppendLine($"{nameof(PostedAt)}: ({PostedAt})");
            sb.AppendLine($"{nameof(TransactedAt)}: ({TransactedAt})");

            return sb.ToString();
        }

        private string AmountString()
        {
            if (Amount < 0.00M)
            {
                return $"Credit of {(0.00M - Amount).ToString("C")}";
            }

            return Amount.ToString("C");
        }
    }
}
