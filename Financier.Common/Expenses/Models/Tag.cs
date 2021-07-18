using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

using Financier.Common.Extensions;

namespace Financier.Common.Expenses.Models
{
    [Table("Expenses_Tags")]
    public class Tag
    {
        public static IList<Tag> GetAll()
        {
            using (var db = new Context())
            {
                return db.Tags
                    .ToList();
            }
        }

        public static Tag Get(string name)
        {
            if (name.IsNullOrWhiteSpace())
            {
                throw new ArgumentException("name cannot be null or whitespace");
            }

            name = name.ToLower().Trim();
            using (var db = new Context())
            {
                return db.Tags
                    .Where(t => t.Name == name)
                    .Include(t => t.ItemTags)
                        .ThenInclude(it => it.Item)
                    .FirstOrDefault();
            }
        }

        public static Tag GetOrCreate(string name)
        {
            if (name.IsNullOrWhiteSpace())
            {
                throw new ArgumentException("name cannot be null or whitespace");
            }

            name = name.ToLower().Trim();
            using (var db = new Context())
            {
                var tag = db.Tags
                    .Where(t => t.Name == name)
                    .Include(t => t.ItemTags)
                        .ThenInclude(it => it.Item)
                    .FirstOrDefault();

                if (tag != null)
                {
                    // Returning an existing tag
                    return tag;
                }

                // Creating a new tag
                tag = new Tag
                {
                    Id = Guid.NewGuid(),
                    Name = name
                };
                db.Tags.Add(tag);
                db.SaveChanges();

                return tag;
            }
        }

        [Key]
        [Required]
        public Guid Id { get; set; }

        private string _name;
        [Required]
        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                _name = value.Trim().ToLower();
            }
        }

        public List<ItemTag> ItemTags { get; set; }

        public List<Item> Items { get; set; }

        public void Delete()
        {
            using (var db = new Context())
            {
                db.Tags.Remove(this);
                db.SaveChanges();
            }
        }

        public Tag Rename(string newName)
        {
            var renamedTag = GetOrCreate(newName);
            // IEnumerable<Guid> existingItemIds;
            using (var db = new Context())
            {
                // TODO: Clean the SQL in case of SQL injection attacks
                // TODO: See how to do it natively using Postgres
                var query = string.Empty
                    + " SELECT itemTags.\"ItemId\" AS \"ItemId\", itemTags.\"TagId\" as \"TagId\""
                    + " FROM \"Expenses_ItemTags\" itemTags"
                    + "   INNER JOIN \"Expenses_Items\" items"
                    + "     ON items.\"Id\" = itemTags.\"ItemId\""
                    + "   INNER JOIN \"Expenses_Tags\" tags"
                    + "     ON tags.\"Id\" = itemTags.\"TagId\""
                    + " WHERE itemTags.\"ItemId\" IN ("
                    + "   SELECT itemTags.\"ItemId\""
                    + "   FROM \"Expenses_ItemTags\" itemTags"
                    + "     INNER JOIN \"Expenses_Tags\" tags"
                    + "       ON tags.\"Id\" = itemTags.\"TagId\""
                    + "     INNER JOIN \"Expenses_Items\" items"
                    + "       ON items.\"Id\" = itemTags.\"ItemId\""
                    + $"  WHERE tags.\"Name\" = '{Name}'"

                    + "   EXCEPT"

                    + "   SELECT itemTags.\"ItemId\""
                    + "   FROM \"Expenses_ItemTags\" itemTags"
                    + "     INNER JOIN \"Expenses_Tags\" tags"
                    + "       ON tags.\"Id\" = itemTags.\"TagId\""
                    + "     INNER JOIN \"Expenses_Items\" items"
                    + "       ON items.\"Id\" = itemTags.\"ItemId\""
                    + $"  WHERE tags.\"Name\" = '{newName}'"
                    + $") AND tags.\"Name\" = '{Name}'";

                var itemTagsToBeUpdated = db.Set<ItemTag>().FromSqlRaw(query).AsEnumerable().ToList();

                var newItemTagsForNewTag = itemTagsToBeUpdated
                    .Select(itemTag => new ItemTag
                    {
                        ItemId = itemTag.ItemId,
                        TagId = renamedTag.Id
                    });

                db.ItemTags.AddRange(newItemTagsForNewTag);
                db.SaveChanges();
            }

            using (var db = new Context())
            {
                db.ItemTags.RemoveRange(ItemTags ?? Enumerable.Empty<ItemTag>());
                db.Tags.Remove(this);
                db.SaveChanges();
            }

            return renamedTag;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var other = obj as Tag;
            if (other == null)
            {
                return false;
            }

            if ((Name ?? string.Empty) != (other.Name ?? string.Empty))
            {
                return false;
            }

            return true;
        }

        public static bool operator ==(Tag x, Tag y)
        {
            if (object.ReferenceEquals(x, null))
            {
                return (object.ReferenceEquals(y, null));
            }

            return x.Equals(y);
        }

        public static bool operator !=(Tag x, Tag y)
        {
            return !(x == y);
        }

        public override string ToString()
        {
            return $"Id = ({Id})\nName = (#{Name})";
        }
    }
}
