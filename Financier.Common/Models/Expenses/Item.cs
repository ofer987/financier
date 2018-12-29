using System;

namespace Financier.Common.Models.Expenses
{
    public class Item
    {
        public Guid Id { get; set; }

        public Guid StatementId { get; set; }

        public Statement Statement { get; set; }

        public string Description { get; set; }

        public decimal Amount { get; set; }

        public DateTime TransactedAt { get; set; }

        public DateTime PostedAt { get; set; }

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
    }
}
