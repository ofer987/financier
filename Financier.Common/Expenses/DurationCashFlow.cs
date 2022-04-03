using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

using Financier.Common.Expenses.Models;
using Financier.Common.Extensions;

// TODO: Remove hardcoded filter tag names and replace with functions

namespace Financier.Common.Expenses
{
    public class DurationCashFlow : CashFlow
    {
        public decimal Threshold { get; protected set; }
        protected const decimal DefaultThreshold = 0.05M;

        private DateTime _startAt;
        public DateTime StartAt
        {
            get { return _startAt; }

            protected set
            {
                _startAt = new DateTime(value.Year, value.Month, value.Day, 0, 0, 0, DateTimeKind.Utc);
            }
        }
        private DateTime _endAt;
        public DateTime EndAt
        {
            get { return _endAt; }

            protected set
            {
                _endAt = new DateTime(value.Year, value.Month, value.Day, 0, 0, 0, DateTimeKind.Utc);
            }
        }

        public int StartYear { get; protected set; }
        public int StartMonth { get; protected set; }
        public int StartDay { get; protected set; }

        public int EndYear { get; protected set; }
        public int EndMonth { get; protected set; }
        public int EndDay { get; protected set; }

        public IReadOnlyList<ItemListing> CreditListings { get; protected set; } = Enumerable.Empty<ItemListing>().ToList();
        public IReadOnlyList<ItemListing> DebitListings { get; protected set; } = Enumerable.Empty<ItemListing>().ToList();

        public IReadOnlyList<ItemListing> CombinedCreditListings { get; protected set; } = Enumerable.Empty<ItemListing>().ToList();
        public IReadOnlyList<ItemListing> CombinedDebitListings { get; protected set; } = Enumerable.Empty<ItemListing>().ToList();

        public decimal CreditAmountTotal { get; protected set; } = 0.00M;
        public decimal DebitAmountTotal { get; protected set; } = 0.00M;
        public decimal ProfitAmountTotal => CreditAmountTotal - DebitAmountTotal;

        public override decimal DailyProfit => decimal.Round(ProfitAmountTotal / EndAt.Subtract(StartAt).Days, 2);

        public DurationCashFlow(string accountName, DateTime startAt, DateTime endAt, decimal threshold = DefaultThreshold) : base(accountName)
        {
            StartAt = startAt;
            EndAt = endAt;
            Threshold = threshold;

            Init();
        }

        protected DurationCashFlow(string accountName) : base(accountName)
        {
        }

        protected virtual void Init()
        {
            SetCredits();
            SetDebits();
        }

        protected void SetCredits()
        {
            CreditListings = GetItemListings(ItemTypes.Credit);
            CombinedCreditListings = CombineItemListings(CreditListings, Threshold);
            CreditAmountTotal = CreditListings
                .Select(cost => cost.Amount)
                .Aggregate(0.00M, (r, i) => r + i);
        }

        protected void SetDebits()
        {
            DebitListings = GetItemListings(ItemTypes.Debit);
            CombinedDebitListings = CombineItemListings(CreditListings, Threshold);
            DebitAmountTotal = DebitListings
                .Select(cost => cost.Amount)
                .Aggregate(0.00M, (r, i) => r + i);
        }

        public IReadOnlyList<ItemListing> GetItemListings(ItemTypes itemType)
        {
            IList<Item> items;
            using (var db = new Context())
            {
                items = db.Items
                    .Include(item => item.Statement)
                        .ThenInclude(stmt => stmt.Card)
                    .Include(item => item.ItemTags)
                        .ThenInclude(it => it.Tag)
                    .Where(item => item.PostedAt >= StartAt)
                    .Where(item => item.PostedAt < EndAt)
                    .Where(item =>
                            item.Statement.Card.AccountName == AccountName
                            && (
                                false
                                || itemType == ItemTypes.Debit && item.Amount >= 0
                                || itemType == ItemTypes.Credit && item.Amount < 0)
                            )
                    .AsEnumerable()
                    .Reject(item => item.Tags.HasInternalTransfer())
                    .ToArray();
            }

            return items
                .GroupBy(item => item.Tags.Select(tag => tag.Name).Join(", "))
                .Select(items => new ItemListing(items.First().Tags, items))
                .ToArray();
        }

        public IReadOnlyList<ItemListing> CombineItemListings(IEnumerable<ItemListing> listings, decimal threshold)
        {
            var totalAmount = listings
                .Select(tc => tc.Amount)
                .Aggregate(0.00M, (r, i) => r + i);

            return GetItemListingsAboveThreshold(listings, totalAmount, threshold)
                .Concat(CombineItemListingsBelowThreshold(listings, totalAmount, threshold))
                .ToList();
        }

        private IEnumerable<ItemListing> GetItemListingsAboveThreshold(IEnumerable<ItemListing> listings, decimal total, decimal threshold)
        {
            return listings
                .Where(tc => tc.Amount / total >= threshold);
        }

        private IEnumerable<ItemListing> CombineItemListingsBelowThreshold(IEnumerable<ItemListing> listings, decimal total, decimal threshold)
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
