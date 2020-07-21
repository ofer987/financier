using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

using Financier.Common.Expenses.Models;
using Financier.Common.Extensions;

// TODO: Remove hardcoded filter tag names and replace with functions
namespace Financier.Common.Expenses
{
    public static class CashFlowHelper
    {
        public static IReadOnlyList<ItemListing> GetItemListings(DateTime startAt, DateTime endAt, ItemTypes itemType)
        {
            IList<Item> items;
            using (var db = new Context())
            {
                items = db.Items
                    .Include(item => item.ItemTags)
                        .ThenInclude(it => it.Tag)
                    .Where(item => item.PostedAt >= startAt)
                    .Where(item => item.PostedAt < endAt)
                    .Where(item =>
                            false
                            || itemType == ItemTypes.Debit && item.Amount >= 0
                            || itemType == ItemTypes.Credit && item.Amount < 0)
                    .Reject(item => item.Tags.HasInternalTransfer())
                    .ToArray();
            }

            return items
                .GroupBy(item => item.Tags.Select(tag => tag.Name).Join(", "))
                .Select(grouped => new ItemListing(grouped.First().Tags, grouped))
                .ToArray();
        }

        public static IReadOnlyList<ItemListing> CombineItemListings(IEnumerable<ItemListing> listings, decimal threshold)
        {
            var totalAmount = listings
                .Select(tc => tc.Amount)
                .Aggregate(0.00M, (r, i) => r + i);

            return GetItemListingsAboveThreshold(listings, totalAmount, threshold)
                .Concat(CombineItemListingsBelowThreshold(listings, totalAmount, threshold))
                .ToList();
        }

        private static IEnumerable<ItemListing> GetItemListingsAboveThreshold(IEnumerable<ItemListing> listings, decimal total, decimal threshold)
        {
            return listings
                .Where(tc => tc.Amount / total >= threshold);
        }

        private static IEnumerable<ItemListing> CombineItemListingsBelowThreshold(IEnumerable<ItemListing> listings, decimal total, decimal threshold)
        {
            var results = new List<ItemListing>();

            var orderedListings = listings
                .Where(tc => tc.Amount / total < threshold)
                .OrderBy(tc => tc.Amount);

            foreach (var listing in orderedListings)
            {
                results.Add(listing);
                var totalAmount = results
                    .Select(result => result.Amount)
                    .Aggregate(0.00M, (r, i) => r + i);

                if (totalAmount / total >= threshold)
                {
                    yield return results
                        .Aggregate(new ItemListing(), (r, i) => r + i);

                    results = new List<ItemListing>();
                }
            }

            if (results.Any())
            {
                yield return results
                    .Aggregate(new ItemListing(), (r, i) => r + i);
            }
        }
    }
}
