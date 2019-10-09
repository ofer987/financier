using System;
using System.Collections.Generic;
using System.Linq;

namespace Financier.Common.Expenses.Models
{
    public class TagAnalysis
    {
        public int Year { get; }
        public int Month { get; }

        public DateTime From { get; }
        public DateTime To { get; }

        public Analysis Analyser { get; }

        public TagAnalysis(int year, int month)
        {
            Year = year;
            Month = month;
            From = new DateTime(year, month, 1);

            if (month == 12)
            {
                To = new DateTime(year + 1, 1, 1).AddDays(-1);
            }
            else
            {
                To = new DateTime(year, month + 1, 1).AddDays(-1);
            }

            Analyser = new Analysis(From, To);
        }

        public IEnumerable<TagCost> GetAssets()
        {
            var tagCosts = Analyser.GetAssetsAndTags()
                .Select(pair => new TagCost(pair.Key, pair.Value));

            return GetGroupedItems(tagCosts);
        }

        public IEnumerable<TagCost> GetExpenses()
        {
            var tagCosts = Analyser.GetExpensesAndTags()
                .Select(pair => new TagCost(pair.Key, pair.Value));

            return GetGroupedItems(tagCosts);
        }

        private IEnumerable<TagCost> GetGroupedItems(IEnumerable<TagCost> tagCosts)
        {
            const decimal threshold = 0.05M;
            var totalAmount = tagCosts
                .Select(tc => tc.Amount)
                .Aggregate(0.00M, (r, i) => r + i);

            return GetAboveThreshold(tagCosts, totalAmount, threshold)
                .Concat(GetBelowThreshold(tagCosts, totalAmount, threshold));
        }

        private IEnumerable<TagCost> GetAboveThreshold(IEnumerable<TagCost> tagCosts, decimal total, decimal threshold)
        {
            return tagCosts
                .Where(tc => tc.Amount / total >= threshold);
        }

        private IEnumerable<TagCost> GetBelowThreshold(IEnumerable<TagCost> tagCosts, decimal total, decimal threshold)
        {
            var results = new List<TagCost>();

            foreach (var tagCost in tagCosts.OrderBy(tc => tc.Amount))
            {
                results.Add(tagCost);
                var totalAmount = results
                    .Select(result => result.Amount)
                    .Aggregate(0.00M, (r, i) => r + i);

                if (totalAmount >= threshold)
                {
                    results = new List<TagCost>();

                    yield return results
                        .Aggregate(new TagCost(), (r, i) => r + i);
                }
            }
        }
    }
}
