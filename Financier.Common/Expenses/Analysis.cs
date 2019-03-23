using System;
using System.Collections.Generic;
using System.Linq;

using Financier.Common.Expenses.Models;

namespace Financier.Common.Expenses
{
    public class Analysis
    {
        public DateTime StartAt { get; }
        public DateTime EndAt { get; }

        public Analysis(DateTime startAt, DateTime endAt)
        {
            StartAt = startAt;
            EndAt = endAt;
        }

        public Dictionary<Tag, List<Item>> ItemsByGroup()
        {
            List<Item> items;
            using (var db = new Context())
            {
                items = db.Items
                    .Where(item => item.TransactedAt >= StartAt)
                    .Where(item => item.TransactedAt < EndAt)
                    .Where(item => item.Amount > 0)
                    .ToList();
            }

            var itemsByTag = new Dictionary<Tag, List<Item>>();
            foreach (var item in items)
            {
                foreach (var tag in item.Tags)
                {
                    if (itemsByTag.TryGetValue(tag, out var taggedItems))
                    {
                        taggedItems.Add(item);
                    }
                    else
                    {
                        itemsByTag.Add(tag, new List<Item> { item });
                    }
                }
            }

            return itemsByTag;
        }

        public decimal GetExpenses()
        {
            using (var db = new Context())
            {
                return db.Items
                    .Where(item => item.TransactedAt >= StartAt)
                    .Where(item => item.TransactedAt < EndAt)
                    .Where(item => item.Amount > 0)
                    .Aggregate(0.00M, (result, item) => result + item.Amount);
            }
        }

        public decimal GetEarnings()
        {
            using (var db = new Context())
            {
                return db.Items
                    .Where(item => item.TransactedAt >= StartAt)
                    .Where(item => item.TransactedAt < EndAt)
                    .Aggregate(0.00M, (result, item) => result + item.Amount);
            }
        }
    }
}
