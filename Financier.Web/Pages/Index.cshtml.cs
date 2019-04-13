using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Financier.Common;
using Financier.Common.Extensions;
using Financier.Common.Expenses;
using Financier.Common.Expenses.Models;

namespace Financier.Web.Pages
{
    public class IndexModel : PageModel
    {
        public DateTime EarliestAt
        {
            get
            {
                DateTime date;
                using (var db = new Context())
                {
                    date = db.Items
                        .OrderBy(item => item.TransactedAt)
                        .First()
                        .TransactedAt;
                }

                return new DateTime(date.Year, date.Month, 1);
            }
        }

        public DateTime LatestAt
        {
            get
            {
                DateTime date;
                using (var db = new Context())
                {
                    date = db.Items
                        .OrderByDescending(item => item.TransactedAt)
                        .First()
                        .TransactedAt;
                }

                return new DateTime(date.Year, date.Month, 1).AddMonths(1).AddDays(-1);
            }
        }

        public IEnumerable<DateTime> Months
        {
            get
            {
                var earliestAt = EarliestAt;
                var latestAt = LatestAt;

                for (var startAt = earliestAt; startAt <= latestAt; startAt = startAt.AddMonths(1))
                {
                    yield return startAt;
                }
            }
        }

        public void OnGet()
        {
            if (ModelState.IsValid)
            {
                Console.WriteLine("Page is valid");
            }
        }

        public IDictionary<IEnumerable<Tag>, decimal> GetAssets(DateTime at)
        {
            const decimal threshold = 0.05M;
            var startAt = at;
            var endAt = at.AddMonths(1).AddDays(-1);
            var total = GetRealAssetTotal(startAt, endAt);
            var amountsByTags = new Analysis(startAt, endAt)
                .GetAssetsAndTags()
                .ToDictionary(pair => pair.Key, pair => pair.Value.Aggregate(0.00M, (r, i) => r - i.Amount))
                .ToDictionary(pair => pair.Key, pair => pair.Value / total);

            return new Dictionary<IEnumerable<Tag>, decimal>()
                .Concat(GetMajorItems(amountsByTags, threshold))
                .Concat(GetMinorItems(amountsByTags, threshold))
                .ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        public IDictionary<IEnumerable<Tag>, decimal> GetExpenses(DateTime at)
        {
            const decimal threshold = 0.05M;
            var startAt = at;
            var endAt = at.AddMonths(1).AddDays(-1);
            var total = GetRealExpenseTotal(startAt, endAt);
            var amountsByTags = new Analysis(startAt, endAt)
                .GetExpensesAndTags()
                .ToDictionary(pair => pair.Key, pair => pair.Value.Aggregate(0.00M, (r, i) => r + i.Amount))
                .ToDictionary(pair => pair.Key, pair => pair.Value / total);

            return new Dictionary<IEnumerable<Tag>, decimal>()
                .Concat(GetMajorItems(amountsByTags, threshold))
                .Concat(GetMinorItems(amountsByTags, threshold))
                .ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        private IDictionary<IEnumerable<Tag>, decimal> GetMajorItems(IDictionary<IEnumerable<Tag>, decimal> items, decimal threshold)
        {
            return items
                .Where(pair => pair.Value >= threshold)
                .ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        private IDictionary<IEnumerable<Tag>, decimal> GetMinorItems(IDictionary<IEnumerable<Tag>, decimal> items, decimal threshold)
        {
            var minorAmounts = items
                .Where(pair => pair.Value < threshold)
                .ToList();

            var results = new Dictionary<IEnumerable<Tag>, decimal>();

            var totalAmount = 0.00M;
            List<Tag> keys = new List<Tag>();

            // skip the last item
            foreach (var amount in minorAmounts.Skip(1).OrderBy(pair => pair.Value))
            {
                totalAmount += amount.Value;
                keys.AddRange(amount.Key);

                if (totalAmount >= threshold)
                {
                    results.Add(keys, totalAmount);
                    keys = new List<Tag>();
                    totalAmount = 0.00M;
                }
            }

            foreach (var amount in minorAmounts.Take(1).OrderBy(pair => pair.Value))
            {
                totalAmount += amount.Value;
                keys.AddRange(amount.Key);

                results.Add(keys, totalAmount);
            }

            return results;
        }

        public decimal GetRealAssetTotal(DateTime startAt, DateTime endAt)
        {
            return new Analysis(startAt, endAt)
                .GetAssetsAndTags()
                .SelectMany(pair => pair.Value)
                .Aggregate(0.00M, (r, i) => r - i.Amount);
        }

        public decimal GetRealExpenseTotal(DateTime startAt, DateTime endAt)
        {
            return new Analysis(startAt, endAt)
                .GetExpensesAndTags()
                .SelectMany(pair => pair.Value)
                .Aggregate(0.00M, (r, i) => r + i.Amount);
        }

        public string DisplayPercentage(decimal amount)
        {
            var specifier = "##.## %";
            return amount.ToString(specifier);
        }

        public string DisplayTagNames(IEnumerable<Tag> tags)
        {
            return tags
                .Select(tag => tag.Name)
                .Distinct()
                .OrderBy(name => name)
                .Join(", ");
        }

        public string DisplayMonth(DateTime at)
        {
            return at.ToString("MMMM yyyy");
        }

        public string DisplayTagIds(IEnumerable<Tag> tags)
        {
            return tags
                .Select(tag => tag.Id)
                .Join(";");
        }
    }
}
