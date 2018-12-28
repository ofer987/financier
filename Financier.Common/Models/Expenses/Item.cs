using System;

namespace Financier.Common.Models.Expenses
{
    public class Item
    {
        public string CardId { get; }

        public string Description { get; }

        public decimal Amount { get; }

        public DateTime TransactedAt { get; }

        public DateTime PostedAt { get; }

        public Item(string cardId, string description, DateTime transactedAt, DateTime postedAt, decimal amount)
        {
            CardId = cardId;
            Description = description;
            TransactedAt = transactedAt;
            PostedAt = postedAt;
            Amount = amount;
        }
    }
}
