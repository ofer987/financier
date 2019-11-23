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
        public static IEnumerable<Item> GetAll()
        {
            using (var db = new Context())
            {
                return db.Items.ToList();
            }
        }

        public static IEnumerable<Item> GetAllBy(DateTime from, DateTime to)
        {
            using (var db = new Context())
            {
                return db.Items
                    .Include(item => item.ItemTags)
                        .ThenInclude(it => it.Tag)
                    .Where(item => item.At >= from)
                    .Where(item => item.At < to)
                    .ToArray();
            }
        }

        public static Item[] GetAllNewItems()
        {
            using (var db = new Context())
            {
                return db.Items
                    .Include(item => item.ItemTags)
                    .Reject(item => item.ItemTags.Any())
                    .OrderBy(item => item.At)
                    .ToArray();
            }
        }

        public static IEnumerable<Item> FindCredits(DateTime from, DateTime to)
        {
            using (var db = new Context())
            {
                return db.Items
                    .Include(item => item.ItemTags)
                        .ThenInclude(it => it.Tag)
                    .Where(item => item.IsCredit)
                    .Where(item => item.At >= from)
                    .Where(item => item.At < to)
                    // .Reject(item => item.Tags.HasCreditCardPayent())
                    .Reject(item => item.Tags.HasInternalTransfer())
                    .ToArray();
            }
        }

        public static IEnumerable<Item> FindDebits(DateTime from, DateTime to)
        {
            using (var db = new Context())
            {
                return db.Items
                    .Include(item => item.ItemTags)
                    .ThenInclude(it => it.Tag)
                    .Where(item => item.IsDebit)
                    .Where(item => item.At >= from)
                    .Where(item => item.At < to)
                    // .Reject(item => item.Tags.HasCreditCardPayent())
                    .Reject(item => item.Tags.HasInternalTransfer())
                    .ToArray();
            }
        }

        public static IEnumerable<Item> FindExternalItems(DateTime from, DateTime to)
        {
            using (var db = new Context())
            {
                return db.Items
                    .Include(item => item.ItemTags)
                    .ThenInclude(it => it.Tag)
                    .Where(item => item.At >= from)
                    .Where(item => item.At < to)
                    // .Reject(item => item.Tags.HasCreditCardPayent())
                    .Reject(item => item.Tags.HasInternalTransfer())
                    .ToArray();
            }
        }

        public static Item Get(Guid id)
        {
            using (var db = new Context())
            {
                return db.Items.First(item => item.Id == id);
            }
        }

        public static Item GetByItemId(string itemId)
        {
            itemId = (itemId ?? string.Empty).Trim();
            using (var db = new Context())
            {
                return db.Items
                    .Include(item => item.ItemTags)
                        .ThenInclude(itemTag => itemTag.Tag)
                    .First(item => item.ItemId == itemId);
            }
        }

        public static IEnumerable<Item> GetByTagIds(IEnumerable<Guid> tagIds)
        {
            using (var db = new Context())
            {
                var query =
                    from item in db.Items
                    join itemTag in db.ItemTags on item.Id equals itemTag.ItemId
                    join tag in db.Tags on itemTag.TagId equals tag.Id
                    where tagIds.Contains(tag.Id)
                    select item;

                return query.ToList();
            }
        }

        public static IEnumerable<Item> GetByTagNames(IEnumerable<string> tagNames)
        {
            using (var db = new Context())
            {
                var query =
                    from item in db.Items
                    join itemTag in db.ItemTags on item.Id equals itemTag.ItemId
                    join tag in db.Tags on itemTag.TagId equals tag.Id
                    where tagNames.Contains(tag.Name)
                    select item;

                return query.ToList();
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

        public virtual decimal TheRealAmount 
        {
            get
            {
                if (IsCredit)
                {
                    return 0.00M - Amount;
                }

                return Amount;
            }
        }

        [Required]
        public DateTime TransactedAt { get; set; }

        [Required]
        public DateTime PostedAt { get; set; }

        // TODO: maybe change to TransactedAt because that is when the transaction took place???
        public DateTime At => PostedAt;

        public List<ItemTag> ItemTags { get; set; } = new List<ItemTag>();

        public IEnumerable<Tag> Tags => ItemTags.Select(it => it.Tag);

        public bool IsDebit => Amount >= 0;
        public bool IsCredit => Amount < 0;

        public Item(Guid id, Guid statementId, string itemId, string description, DateTime at, decimal amount)
        {
            Id = id;
            StatementId = statementId;
            ItemId = itemId;
            Description = description;
            TransactedAt = at;
            PostedAt = at;
            Amount = amount;
        }

        public Item(Guid id, Guid statementId, string itemId, string description, DateTime transactedAt, DateTime postedAt, decimal amount)
        {
            Id = id;
            StatementId = statementId;
            ItemId = itemId;
            Description = description;
            TransactedAt = transactedAt;
            PostedAt = postedAt;
            Amount = amount;
        }

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
                    || TheRealAmount != other.TheRealAmount
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
            sb.AppendLine($"{nameof(TheRealAmount)}: ({AmountString()})");
            sb.AppendLine($"{nameof(Amount)}: ({Amount})");
            sb.AppendLine($"{nameof(PostedAt)}: ({PostedAt})");
            sb.AppendLine($"{nameof(TransactedAt)}: ({TransactedAt})");

            return sb.ToString();
        }

        private string AmountString()
        {
            if (IsCredit)
            {
                return $"Credit of {(TheRealAmount).ToString("C")}";
            }

            return TheRealAmount.ToString("C");
        }
    }
}
