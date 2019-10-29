using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

using Financier.Common.Extensions;

namespace Financier.Common.Expenses.Models
{
    public class ItemQuery
    {
        public IEnumerable<Tag> Tags { get; }
        public DateTime From { get; }
        public DateTime To { get; }
        public bool IsAsset { get; }

        // public decimal Amount => Items.Aggregate(0.00M, (r, i) => r + i.TheRealAmount);

        public ItemQuery(IEnumerable<Tag> tags, DateTime fro, DateTime to, bool isAsset)
        {
            From = fro;
            To = to;
            Tags = tags;
            IsAsset = isAsset;

            Validate();
        }

        public void Validate()
        {
            if (From >= To)
            {
                throw new ArgumentOutOfRangeException(nameof(From), $"Should be before {nameof(To)}");
            }

            if (Tags == null)
            {
                throw new ArgumentNullException(nameof(Tags));
            }
        }

        public IEnumerable<Item> Query()
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
                        && Tags.Any(tag => tag.Name == t.Name)
                    select i
                )
                .Include(item => item.ItemTags)
                    .ThenInclude(itemTag => itemTag.Tag)
                .ToArray();
            }

            return items
                .Reject(item => item.Tags.HasInternalTransfer())
                .ToArray();
        }

    }
}
