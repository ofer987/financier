using System;
using System.Collections.Generic;
using System.Linq;
using McMaster.Extensions.CommandLineUtils;

using Financier.Common;
using Financier.Common.Expenses;
using Financier.Common.Expenses.Models;
using Financier.Common.Extensions;

namespace Financier.Cli.Listings
{
    public class Program
    {
        public static void Main(string[] args) => CommandLineApplication.Execute<Program>(args);

        private DateTime EarliestAt
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

        private DateTime LatestAt
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

        private void OnExecute()
        {
            Context.Environment = Environments.Dev;

            var earliestAt = EarliestAt;
            var latestAt = LatestAt;

            Console.WriteLine("Cash Flow");
            var startAt = earliestAt;
            while (startAt <= latestAt)
            {
                var endAt = startAt.AddMonths(1).AddDays(-1);
                var items = new Analysis(startAt, endAt).GetAllExpenses();

                var amount = items.Aggregate(0.00M, (result, item) => result + item.Amount);
                Console.WriteLine($"\t{startAt.ToString("MMMM yyyy")}\t{amount}");

                startAt = startAt.AddMonths(1);
            }

            Console.WriteLine();
            Console.WriteLine("Expenditures");
            startAt = earliestAt;
            while (startAt <= latestAt)
            {
                var endAt = startAt.AddMonths(1).AddDays(-1);

                Console.WriteLine($"\t{startAt.ToString("MMMM yyyy")}");
                DisplayItemsByTag(startAt, endAt);

                startAt = endAt;
            }

            Console.WriteLine();
            Console.WriteLine("Condensed CashFlow Statement");
            startAt = earliestAt;
            while (startAt <= latestAt)
            {
                var endAt = startAt.AddMonths(1).AddDays(-1);
                var profit = GetRealAssetTotal(startAt, endAt) - GetRealExpenseTotal(startAt, endAt);

                Console.WriteLine($"\t{startAt.ToString("MMMM yyyy")} ({profit})");
                Console.WriteLine($"\tAssets ({GetRealAssetTotal(startAt, endAt)})");
                DisplayOrderedAssetPercentageAndTag(startAt, endAt);

                Console.WriteLine($"\tExpenses ({GetRealExpenseTotal(startAt, endAt)})");
                DisplayOrderedExpensePercentageAndTag(startAt, endAt);

                startAt = startAt.AddMonths(1);
            }
        }

        private void DisplayItemsByTag(DateTime startAt, DateTime endAt)
        {
            foreach (var result in new Analysis(startAt, endAt).GetItemsByTag().GroupBy(result => result.Item1))
            {
                var tagName = result.Key;
                var tag = result.Key;
                var itemAndTags = result;

                var total = result.Aggregate(0.00M, (r, i) => r + i.Item2.Amount);
                Console.WriteLine($"\t\t{tag.Name} for a total of {total}");
                foreach (var item in itemAndTags)
                {
                    if (item.Item2.Amount >= 0)
                    {
                        Console.WriteLine($"\t\t\t{item.Item2.Description} bought for {item.Item2.Amount} dollars");
                    }
                    else
                    {
                        Console.WriteLine($"\t\t\t{item.Item2.Description} earned {0.00M - item.Item2.Amount} dollars");
                    }
                }
            }
        }

        private void DisplayOrderedAmountByTag(DateTime startAt, DateTime endAt)
        {
            var amountsByTags = new Analysis(startAt, endAt)
                .GetItemsByTag()
                .GroupBy(result => result.Item1)
                .Select(i => ValueTuple.Create<Tag, decimal>(i.Key, i.Aggregate(0.00M, (r, item) => r + item.Item2.Amount)))
                .OrderByDescending(amount => amount.Item2);

            foreach (var amount in amountsByTags)
            {
                Console.WriteLine($"\t\t{amount.Item1.Name} for a total of {amount.Item2}");
            }
        }

        private void DisplayOrderedAssetsPercentageByTag(DateTime startAt, DateTime endAt)
        {
            var total = GetAssetTotal(startAt, endAt);
            var amountsByTags = new Analysis(startAt, endAt)
                .GetAssetsByTag()
                .ToDictionary(pair => pair.Key, pair => pair.Value.Aggregate(0.00M, (r, i) => r + i.Amount))
                .OrderBy(pair => pair.Value);

            foreach (var pair in amountsByTags)
            {
                var percentage = pair.Value / total;

                Console.WriteLine($"\t\t{pair.Key.Name} for a total of {percentage.ToString("P")}");
            }
        }

        private void DisplayOrderedPercentageByTag(DateTime startAt, DateTime endAt)
        {
            var total = GetExpenseTotal(startAt, endAt);
            var amountsByTags = new Analysis(startAt, endAt)
                .GetExpensesByTag()
                .ToDictionary(pair => pair.Key, pair => pair.Value.Aggregate(0.00M, (r, i) => r + i.Amount))
                .OrderByDescending(pair => pair.Value);

            foreach (var pair in amountsByTags)
            {
                var percentage = pair.Value / total;

                Console.WriteLine($"\t\t{pair.Key.Name} for a total of {percentage.ToString("P")}");
            }
        }

        private void DisplayOrderedAssetPercentageAndTag(DateTime startAt, DateTime endAt)
        {
            var total = GetRealAssetTotal(startAt, endAt);
            var amountsByTags = new Analysis(startAt, endAt)
                .GetAssetsAndTags()
                .ToDictionary(pair => pair.Key, pair => pair.Value.Aggregate(0.00M, (r, i) => r - i.Amount))
                .ToDictionary(pair => pair.Key, pair => pair.Value / total);

            DisplayItems(amountsByTags);
        }

        private void DisplayItems(IDictionary<IEnumerable<Tag>, decimal> amountsByTags)
        {
            const decimal threshold = 0.05M;

            var processedItems = GetMajorItems(amountsByTags, threshold)
                .Concat(GetMinorItems(amountsByTags, threshold))
                .OrderByDescending(pair => pair.Value);

            foreach (var pair in processedItems)
            {
                var tagNames = pair.Key.Select(tag => tag.Name).Distinct().OrderBy(name => name).Join(", ");
                var percentage = pair.Value;

                Console.WriteLine($"\t\t{tagNames} for a total of {percentage.ToString("P")}");
            }
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

        private void DisplayOrderedExpensePercentageAndTag(DateTime startAt, DateTime endAt)
        {
            var total = GetRealExpenseTotal(startAt, endAt);
            var amountsByTags = new Analysis(startAt, endAt)
                .GetExpensesAndTags()
                .ToDictionary(pair => pair.Key, pair => pair.Value.Aggregate(0.00M, (r, i) => r + i.Amount))
                .ToDictionary(pair => pair.Key, pair => pair.Value / total);

            DisplayItems(amountsByTags);
        }

        private decimal GetExpenseTotal(DateTime startAt, DateTime endAt)
        {
            return new Analysis(startAt, endAt)
                .GetExpensesByTag()
                .SelectMany(pair => pair.Value)
                .Aggregate(0.00M, (r, i) => r + i.Amount);
        }

        private decimal GetAssetTotal(DateTime startAt, DateTime endAt)
        {
            return new Analysis(startAt, endAt)
                .GetAssetsByTag()
                .SelectMany(pair => pair.Value)
                .Aggregate(0.00M, (r, i) => r + i.Amount);
        }

        private decimal GetRealAssetTotal(DateTime startAt, DateTime endAt)
        {
            return new Analysis(startAt, endAt)
                .GetAssetsAndTags()
                .SelectMany(pair => pair.Value)
                .Aggregate(0.00M, (r, i) => r - i.Amount);
        }

        private decimal GetRealExpenseTotal(DateTime startAt, DateTime endAt)
        {
            return new Analysis(startAt, endAt)
                .GetExpensesAndTags()
                .SelectMany(pair => pair.Value)
                .Aggregate(0.00M, (r, i) => r + i.Amount);
        }
    }
}
