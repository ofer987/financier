using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Linq;

using Financier.Common.Extensions;

namespace Financier.Common.Expenses.Models
{
    [Table("Expenses_Tags")]
    public class Tag
    {
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
                    .DefaultIfEmpty(null)
                    .FirstOrDefault(t => t.Name == name);

                if (tag != null)
                {
                    return tag;
                }

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

        public IEnumerable<Item> Items => ItemTags.Select(it => it.Item);

        public void Delete()
        {
            using (var db = new Context())
            {
                db.Tags.Remove(this);
                db.SaveChanges();
            }
        }

        public void Rename(string newName)
        {
            var renamedTag = new Tag { Name = newName };
            using (var db = new Context())
            {
                var existingRenamedTag = db.Tags
                    .DefaultIfEmpty(null)
                    .FirstOrDefault(tag => tag.Name == renamedTag.Name);
                if (existingRenamedTag == null)
                {
                    db.Tags
                        .First(t => t.Name == Name)
                        .Name = newName;
                    db.SaveChanges();
                }
                else
                {
                    // Transfer existing ItemTags of this Tag to the other one
                    // TODO: can we use the `ItemTags` property?
                    var existingItemTags =
                        from itemTags in db.ItemTags
                        join tags in db.Tags on itemTags.TagId equals tags.Id
                        where tags.Name == Name
                        select itemTags;
                    foreach (var itemTag in existingItemTags)
                    {
                        // Can I just use existingRenamedTag?
                        var itemsThatHaveNewName =
                            from itemTags in db.ItemTags
                            join tags in db.Tags on itemTags.TagId equals tags.Id
                            join items in db.Items on itemTags.ItemId equals items.Id
                            where tags.Name == renamedTag.Name
                            select items;
                        if (itemsThatHaveNewName.Select(item => item.Id).Contains(itemTag.ItemId))
                        {
                            continue;
                        }

                        db.ItemTags.Add(new ItemTag { ItemId = itemTag.ItemId, TagId = existingRenamedTag.Id });
                    }
                    db.SaveChanges();

                    Delete();
                }
            }
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
