using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

using Financier.Common.Expenses.Models;
using Financier.Common.Extensions;

namespace Financier.Common.Expenses
{
    // TODO Rename to MonthlyCashFlow
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
            Validate(startAt, endAt);
            Init(startAt, endAt);
        }

        public IMonthlyListing GetMonthlyListing(int year, int month)
        {
            var at = new DateTime(year, month, 1);
            if (at > this.EndAt)
            {
                throw new ArgumentException($"Can only display past debits and credits, i.e., before {this.EndAt}");
            }

            var creditsExist = true;
            decimal creditAmount;
            if (!this.CreditListings.TryGetValue((year, month), out creditAmount))
            {
                creditsExist = false;
                creditAmount = 0.00M;
            }

            var debitsExist = true;
            decimal debitAmount;
            if (!this.DebitListings.TryGetValue((year, month), out debitAmount))
            {
                debitsExist = false;
                debitAmount = 0.00M;
            }

            if (!creditsExist && !debitsExist)
            {
                return new NullMonthlyListing();
            }

            return new ExistingMonthlyListing
            {
                Year = year,
                Month = month,
                Credit = creditAmount,
                Debit = debitAmount
            };
        }

        public IMonthlyListing GetProjectedMonthlyListing(int year, int month)
        {
            var at = new DateTime(year, month, 1);
            if (at < this.EndAt)
            {
                throw new ArgumentException($"Can only project debits and credits after {this.EndAt.AddDays(-1)}");
            }

            return new PredictionMonthlyListing
            {
                Year = year,
                Month = month,
                Credit = this.AverageCreditAmount,
                Debit = this.AverageDebitAmount
            };
        }

        private void Validate(DateTime startAt, DateTime endAt)
        {
            if (startAt.AddMonths(1) > endAt)
            {
                throw new ArgumentException($"EndAt {endAt} should be at least one month ahead of StartAt ({startAt})", nameof(endAt));
            }
        }

        private void Init(DateTime startAt, DateTime endAt)
        {
            this.CreditListings = GetItems(ItemTypes.Credit, startAt, endAt);
            this.DebitListings = GetItems(ItemTypes.Debit, startAt, endAt);

            this.TotalCreditAmount = this.CreditListings
                .Select(listing => listing.Value)
                .Aggregate(0.00M, (total, amount) => total + amount);

            this.TotalDebitAmount = this.DebitListings
                .Select(listing => listing.Value)
                .Aggregate(0.00M, (total, amount) => total + amount);

            this.StartAt = this.CreditListings.Concat(this.DebitListings)
                .Select(item => new DateTime(item.Key.Item1, item.Key.Item2, 1))
                .OrderBy(item => item)
                .DefaultIfEmpty(startAt)
                .FirstOrDefault();

            this.EndAt = this.CreditListings.Concat(this.DebitListings)
                .Select(item => new DateTime(item.Key.Item1, item.Key.Item2, 1).AddMonths(1))
                .OrderBy(item => item)
                .DefaultIfEmpty(endAt)
                .LastOrDefault();

            var monthsApart = 0
                + (this.EndAt.Year * 12 - this.StartAt.Year * 12)
                + (this.EndAt.Month - this.StartAt.Month);

            this.AverageCreditAmount = this.TotalCreditAmount / monthsApart;
            this.AverageDebitAmount = this.TotalDebitAmount / monthsApart;
        }

        private IDictionary<(int, int), decimal> GetItems(ItemTypes itemType, DateTime startAt, DateTime endAt)
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
