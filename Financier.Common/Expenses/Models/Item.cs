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
                return db.Items.AsEnumerable().ToList();
            }
        }

        public static IEnumerable<DateTime> GetAllMonths()
        {
            using (var db = new Context())
            {
                return db.Items
                    .Select(item => new Tuple<int, int>(item.PostedAt.Year, item.PostedAt.Month))
                    .Distinct()
                    .AsEnumerable()
                    .Select(item => new DateTime(item.Item1, item.Item2, 1))
                    .OrderBy(postedAt => postedAt)
                    .ToArray();
            }
        }

        public static IEnumerable<Item> GetAllBy(DateTime from, DateTime to)
        {
            using (var db = new Context())
            {
                return db.Items
                    .Include(item => item.ItemTags)
                        .ThenInclude(it => it.Tag)
                    .AsEnumerable()
                    .Where(item => item.PostedAt >= from)
                    .Where(item => item.PostedAt < to)
                    .ToArray();
            }
        }

        public static Item[] GetAllNewItems()
        {
            using (var db = new Context())
            {
                return db.Items
                    .Include(item => item.ItemTags)
                    .AsEnumerable()
                    .Reject(item => item.ItemTags.Any())
                    .OrderBy(item => item.PostedAt)
                    .ToArray();
            }
        }

        public static Item[] FindByDescription(string description)
        {
            using (var db = new Context())
            {
                return db.Items
                    .Include(item => item.ItemTags)
                        .ThenInclude(it => it.Tag)
                    .Where(item => item.Description.Equals(description, StringComparison.InvariantCultureIgnoreCase))
                    .AsEnumerable()
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
                    .AsEnumerable()
                    .Where(item => item.PostedAt >= from)
                    .Where(item => item.PostedAt < to)
                    .Where(item => item.Amount < 0)
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
                    .AsEnumerable()
                    .Where(item => item.PostedAt >= from)
                    .Where(item => item.PostedAt < to)
                    .Where(item => item.Amount >= 0)
                    .Reject(item => item.Tags.HasCreditCardPayment())
                    .Reject(item => item.Tags.HasInternalTransfer())
                    .ToArray();
            }
        }

        public static IEnumerable<Item> FindExternalItems()
        {
            using (var db = new Context())
            {
                return db.Items
                    .Include(item => item.ItemTags)
                        .ThenInclude(it => it.Tag)
                    .AsEnumerable()
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
                    .AsEnumerable()
                    .Where(item => item.PostedAt >= from)
                    .Where(item => item.PostedAt < to)
                    .Reject(item => item.Tags.HasCreditCardPayment())
                    .Reject(item => item.Tags.HasInternalTransfer())
                    .ToArray();
            }
        }

        public static Item Get(Guid id)
        {
            using (var db = new Context())
            {
                return db.Items
                    .Include(item => item.ItemTags)
                        .ThenInclude(it => it.Tag)
                    .First(item => item.Id == id);
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
                    .AsEnumerable()
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

                return query.AsEnumerable().ToList();
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

                return query.AsEnumerable().ToList();
            }
        }

        public ItemTypes Type => Amount >= 0
            ? ItemTypes.Debit
            : ItemTypes.Credit;

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
                if (Type == ItemTypes.Credit)
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

        public List<ItemTag> ItemTags { get; set; }

        public List<Tag> Tags { get; set; }

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

        public void AddTags(string commaSeparatedNewTagNames)
        {
            var names = commaSeparatedNewTagNames
                .Split(',')
                .Select(item => item.Trim().ToLower())
                .Reject(item => item.IsNullOrEmpty())
                .Distinct();

            AddTags(names);
        }

        public void AddTags(IEnumerable<string> newTagNames)
        {
            var tags = newTagNames.Select(Tag.GetOrCreate);

            AddTags(tags);
        }

        public void AddTags(IEnumerable<Tag> newTags)
        {
            using (var db = new Context())
            {
                var existingItemTags = db.ItemTags
                    .Include(it => it.Tag)
                    .Where(it => it.ItemId == Id);
                var existingTags = existingItemTags.Select(it => it.Tag);

                foreach (var newTag in newTags)
                {
                    if (!existingTags.Any(tag => tag.Name == newTag.Name))
                    {
                        var itemTag = new ItemTag
                        {
                            ItemId = Id,
                            TagId = newTag.Id
                        };

                        db.ItemTags.Add(itemTag);
                    }
                }

                db.SaveChanges();
            }
        }

        public void UpdateTags(IEnumerable<string> newTagNames)
        {
            var newTags = newTagNames.Select(name => Tag.GetOrCreate(name));

            UpdateTags(newTags);
        }

        public void UpdateTags(IEnumerable<Tag> newTags)
        {
            using (var db = new Context())
            {
                // TODO: Replace with AddTags
                // Get the existing tags
                var existingItemTags = db.ItemTags
                    .Include(it => it.Tag)
                    .Where(it => it.ItemId == Id);
                var existingTags = existingItemTags.Select(it => it.Tag).AsEnumerable();

                // Add tags that do not exist
                foreach (var newTag in newTags)
                {
                    if (!existingTags.Any(tag => tag.Name == newTag.Name))
                    {
                        var itemTag = new ItemTag
                        {
                            ItemId = Id,
                            TagId = newTag.Id
                        };

                        db.ItemTags.Add(itemTag);
                    }
                }

                // Delete the existing tags that are not part of newTags
                // TODO: See if it is possible to apply the same logic to Tag.Rename
                var itemTagsToDelete = existingItemTags
                    .Reject(existingItemTag => newTags.Any(newTag => newTag.Name == existingItemTag.Tag.Name));
                db.ItemTags.RemoveRange(itemTagsToDelete);

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
            sb.AppendLine($"{nameof(ItemId)}: ({ItemId})");
            sb.AppendLine($"{nameof(Description)}: ({Description ?? string.Empty})");
            sb.AppendLine($"{nameof(TheRealAmount)}: ({AmountString()})");
            sb.AppendLine($"{nameof(Amount)}: ({Amount})");
            sb.AppendLine($"{nameof(PostedAt)}: ({PostedAt})");
            sb.AppendLine($"{nameof(TransactedAt)}: ({TransactedAt})");

            return sb.ToString();
        }

        private string AmountString()
        {
            if (Type == ItemTypes.Credit)
            {
                return $"Credit of {(TheRealAmount).ToString("C")}";
            }

            return TheRealAmount.ToString("C");
        }
    }
}
