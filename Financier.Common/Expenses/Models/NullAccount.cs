using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace Financier.Common.Expenses.Models
{
    public class NullAccount : IAccount
    {
        [Key]
        [Required]
        // TODO: make unique
        public string Name { get; set; } = string.Empty;

        public List<Card> Cards { get; set; } = new List<Card>();

        // TODO: write tests
        public IEnumerable<Item> GetAllItems(DateTime from, DateTime to)
        {
            return Enumerable.Empty<Item>();
        }

        public void Delete()
        {
        }

        public override string ToString()
        {
            return "Null Account";
        }
    }
}
