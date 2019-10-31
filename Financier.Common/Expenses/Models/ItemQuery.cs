using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

using Financier.Common.Extensions;

namespace Financier.Common.Expenses.Models
{
    public class ItemQuery
    {
        public IEnumerable<string> TagNames { get; }
        public DateTime From { get; }
        public DateTime To { get; }
        public bool IsAsset { get; }

        public ItemQuery(IEnumerable<string> tagNames, DateTime fro, DateTime to, bool isAsset)
        {
            From = fro;
            To = to;
            TagNames = tagNames;
            IsAsset = isAsset;

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
                    join it in db.ItemTags on i.Id equals it.ItemId
                    join t in db.Tags on it.TagId equals t.Id
                    where true
                        && i.At >= From
                        && i.At < To
                        && (IsAsset && i.IsCredit || !IsAsset && i.IsDebit)
                        && TagNames.Any(tagName => tagName == t.Name)
                    select i
                )
                .Include(item => item.ItemTags)
                    .ThenInclude(itemTag => itemTag.Tag)
                .ToArray();
            }

            var externalItems = items
                .Reject(item => item.Tags.HasInternalTransfer())
                .ToArray();

            return new ItemResult(this, externalItems);
        }

        public IEnumerable<MonthlyItemResult> GetResultsOrderedByMonth()
        {
            var monthlyItems = GetResults().Items
                .GroupBy(item => new DateTime(item.At.Year, item.At.Month, 1))
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
