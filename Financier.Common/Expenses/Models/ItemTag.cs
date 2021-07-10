using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Financier.Common.Expenses.Models
{
    [Table("Expenses_ItemTags")]
    public class ItemTag
    {
        public Guid ItemId { get; set; }
        public Item Item { get; set; }

        public Guid TagId { get; set; }
        public Tag Tag { get; set; }

        public void Delete()
        {
            using (var db = new Context())
            {
                db.ItemTags.Remove(this);
                db.SaveChanges();
            }
        }
    }
}
