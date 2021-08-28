using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

using Financier.Common.Expenses.Models;
using Financier.Common.Extensions;

namespace Financier.Common.Expenses
{
    public class ItemQuery
    {
        public string AccountName { get; }
        public IEnumerable<string> TagNames { get; }
        public DateTime From { get; }
        public DateTime To { get; }
        public ItemTypes ItemType { get; }

        public ItemQuery(string accountName, IEnumerable<string> tagNames, DateTime fro, DateTime to, ItemTypes itemType)
        {
            AccountName = accountName;
            From = fro;
            To = to;
            TagNames = tagNames.Select(name => name.ToLower());
            ItemType = itemType;

            Validate();
        }

        public void Validate()
        {
            if (From >= To)
            {
                throw new ArgumentOutOfRangeException(nameof(From), $"Should be before {nameof(To)}");
            }

            if (TagNames == null)
            {
                throw new ArgumentNullException(nameof(TagNames));
            }
        }

        public ItemResult GetResults()
        {
            Item[] items;
            using (var db = new Context())
            {
                items = (
                    from i in db.Items
                    join s in db.Statements on i.StatementId equals s.Id
                    join c in db.Cards on s.CardId equals c.Id
                    join a in db.Accounts on c.AccountName equals a.Name
                    join it in db.ItemTags on i.Id equals it.ItemId
                    join t in db.Tags on it.TagId equals t.Id
                    where true
                        && a.Name == AccountName
                        && i.PostedAt >= From
                        && i.PostedAt < To
                        && (
                            ItemType == ItemTypes.Debit && i.Amount >= 0
                            || ItemType == ItemTypes.Credit && i.Amount < 0
                        )
                        && TagNames.Any(tagName => tagName == t.Name)
                    select i
                )
                .Include(item => item.ItemTags)
                    .ThenInclude(itemTag => itemTag.Tag)
                .Include(item => item.Statement)
                    .ThenInclude(s => s.Card)
                .AsEnumerable()
                .ToArray();
            }

            var externalItems = items
                .Reject(item => item.Tags.HasInternalTransfer())
                .Distinct()
                .ToArray();

            return new ItemResult(this, externalItems);
        }

        public IEnumerable<MonthlyItemResult> GetResultsOrderedByMonth()
        {
            var monthlyItems = GetResults().Items
                .GroupBy(item => new DateTime(item.PostedAt.Year, item.PostedAt.Month, 1))
                .ToDictionary(items => items.Key, items => items.AsEnumerable());

            var startAt = new DateTime(From.Year, From.Month, 1);
            var endAt = new DateTime(To.Year, To.Month, 1);
            for (var at = startAt; at <= endAt; at = at.AddMonths(1))
            {
                yield return new MonthlyItemResult(
                    this,
                    monthlyItems.GetValueOrDefault(at, Enumerable.Empty<Item>()),
                    at
                );
            }
        }

    }
}
