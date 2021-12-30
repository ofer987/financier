using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

using Financier.Common.Expenses.Models;
using Financier.Common.Extensions;

namespace Financier.Common.Expenses
{
    public class ProjectedCashFlow
    {
        public DateTime StartAt { get; protected set; }
        public DateTime EndAt { get; protected set; }

        public IDictionary<(int, int), decimal> CreditListings { get; protected set; } = new Dictionary<(int, int), decimal>();
        public IDictionary<(int, int), decimal> DebitListings { get; protected set; } = new Dictionary<(int, int), decimal>();

        public decimal TotalCreditAmount { get; private set; }
        public decimal TotalDebitAmount { get; private set; }

        public decimal AverageCreditAmount { get; private set; }
        public decimal AverageDebitAmount { get; private set; }

        public ProjectedCashFlow(DateTime startAt, DateTime endAt)
        {
            StartAt = startAt;
            EndAt = endAt;

            Validate();
            Init();
        }

        public MonthlyListing GetMonthlyListing(int year, int month)
        {
            var at = new DateTime(year, month, 1);
            if (at > this.EndAt)
            {
                throw new ArgumentException($"Can only display past debits and credits, i.e., before {this.EndAt}");
            }
            var creditAmount = this.CreditListings[(year, month)];
            var debitAmount = this.DebitListings[(year, month)];

            return new MonthlyListing
            {
                Year = year,
                Month = month,
                Credit = creditAmount,
                Debit = debitAmount
            };
        }

        public MonthlyListing GetProjectedMonthlyListing(int year, int month)
        {
            var at = new DateTime(year, month, 1);
            if (at < this.EndAt)
            {
                throw new ArgumentException($"Can only project debits and credits after {this.EndAt.AddDays(-1)}");
            }

            return new MonthlyListing
            {
                Year = year,
                Month = month,
                Credit = this.AverageCreditAmount,
                Debit = this.AverageDebitAmount
            };
        }

        protected void Validate()
        {
            if (this.StartAt.AddMonths(1) > this.EndAt)
            {
                throw new ArgumentException($"EndAt {this.EndAt} should be at least one month ahead of StartAt ({this.StartAt})", nameof(this.EndAt));
            }
        }

        protected void Init()
        {
            this.CreditListings = GetItems(ItemTypes.Credit);
            this.DebitListings = GetItems(ItemTypes.Debit);

            this.TotalCreditAmount = this.CreditListings
                .Select(listing => listing.Value)
                .Aggregate(0.00M, (total, amount) => total + amount);

            this.TotalDebitAmount = this.DebitListings
                .Select(listing => listing.Value)
                .Aggregate(0.00M, (total, amount) => total + amount);

            var monthsApart = 0
                + (this.EndAt.Year * 12 - this.StartAt.Year * 12)
                + (this.EndAt.Month - this.StartAt.Month);

            this.AverageCreditAmount = this.TotalCreditAmount / monthsApart;
            this.AverageDebitAmount = this.TotalDebitAmount / monthsApart;
        }

        private IDictionary<(int, int), decimal> GetItems(ItemTypes itemType)
        {
            IList<Item> items;
            using (var db = new Context())
            {
                items = db.Items
                    .Include(item => item.ItemTags)
                        .ThenInclude(it => it.Tag)
                    .Where(item => item.PostedAt >= StartAt)
                    .Where(item => item.PostedAt < EndAt)
                    .Where(item =>
                        false
                        || itemType == ItemTypes.Debit && item.Amount >= 0
                        || itemType == ItemTypes.Credit && item.Amount < 0)
                    .AsEnumerable()
                    .Reject(item => item.Tags.HasInternalTransfer())
                    .ToArray();
            }

            var results = items
                .GroupBy(item => (item.PostedAt.Year, item.PostedAt.Month), item => item.TheRealAmount)
                .ToDictionary(item => item.Key, item => item.Aggregate(0.00M, (total, amount) => total + amount));

            return results;
        }
    }
}
