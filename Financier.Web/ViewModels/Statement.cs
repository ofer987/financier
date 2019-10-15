using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

using Financier.Common.Expenses;
using Financier.Common.Expenses.Models;

namespace Financier.Web.ViewModels
{
    public class Statement
    {
        public int Year { get; }
        public int Month { get; }
        public DateTime At => new DateTime(Year, Month, 1);

        public DateTime From { get; }
        public DateTime To { get; }

        public IEnumerable<TagCost> AssetCosts { get; private set; } = Enumerable.Empty<TagCost>();
        public IEnumerable<TagCost> ExpenseCosts { get; private set; } = Enumerable.Empty<TagCost>();

        public IEnumerable<TagCost> GroupedAssetCosts { get; private set; } = Enumerable.Empty<TagCost>();
        public IEnumerable<TagCost> GroupedExpenseCosts { get; private set; } = Enumerable.Empty<TagCost>();

        public decimal AssetAmountTotal { get; private set; } = 0.00M;
        public decimal ExpenseAmountTotal { get; private set; } = 0.00M;

        public Statement(int year, int month)
        {
            Year = year;
            Month = month;
            From = new DateTime(year, month, 1);
            
            if (month == 12)
            {
                To = new DateTime(year + 1, 1, 1);
            }
            else
            {
                To = new DateTime(year, month + 1, 1);
            }

            ProcessCosts();
        }

        public Statement(DateTime fro, DateTime to)
        {
            if (fro >= to)
            {
                throw new Exception($"the {nameof(fro)} ({fro}) has to be before {nameof(to)} ({to})");
            }

            From = fro;
            To = to;

            ProcessCosts();
        }

        private decimal? expenseTotal = null;
        public decimal GetExpenseTotal()
        {
            if (!expenseTotal.HasValue)
            {
                expenseTotal = Financier.Common.Expenses.Models.Item
                    .FindDebits(From, To)
                    .Sum(item => item.Amount);
                // return db.Items
                //     .Where(item => item.Amount > 0)
                //     .Where(item => item.TransactedAt >= From)
                //     .Where(item => item.TransactedAt < To)
                //     .Sum(item => item.Amount);
            }

            return expenseTotal.Value;
        }

        private decimal? assetTotal = null;
        public decimal GetAssetTotal()
        {
            if (!assetTotal.HasValue)
            {
                assetTotal = Financier.Common.Expenses.Models.Item
                    .FindCredits(From, To)
                    .Sum(item => item.Amount);
                // using (var db = new Context())
                // {
                //     assetTotal = 0 - db.Items
                //         .Where(item => item.Amount < 0)
                //         .Where(item => item.TransactedAt >= From)
                //         .Where(item => item.TransactedAt < To)
                //         .Sum(item => item.Amount);
                // }
            }

            return assetTotal.Value;
        }

        public decimal GetProfitTotal()
        {
            return GetAssetTotal() - GetExpenseTotal();
        }

        public IEnumerable<Financier.Common.Expenses.Models.Item> GetItems()
        {
            return Financier.Common.Expenses.Models.Item.FindExternalItems(From, To);
        }

        public IEnumerable<TagCost> GetTagAssets()
        {
            return new Analysis(From, To).GetTagAssets();
        }

        public void ProcessCosts()
        {
            SetAssets();
            SetExpenses();
        }

        private void SetAssets()
        {
            AssetCosts = new Analysis(From, To).GetTagAssets();
            GroupedAssetCosts = GetGroupedItems(AssetCosts);
            AssetAmountTotal = AssetCosts
                .Select(cost => cost.Amount)
                .Aggregate(0.00M, (r, i) => r + i);
        }

        private void SetExpenses()
        {
            ExpenseCosts = new Analysis(From, To).GetTagExpenses();
            GroupedExpenseCosts = GetGroupedItems(ExpenseCosts);
            ExpenseAmountTotal = ExpenseCosts
                .Select(cost => cost.Amount)
                .Aggregate(0.00M, (r, i) => r + i);
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

            var orderedItems = tagCosts
                .Where(tc => tc.Amount / total < threshold)
                .OrderBy(tc => tc.Amount);

            foreach (var tagCost in orderedItems)
            {
                results.Add(tagCost);
                var totalAmount = results
                    .Select(result => result.Amount)
                    .Aggregate(0.00M, (r, i) => r + i);

                if (totalAmount / total >= threshold)
                {
                    yield return results
                        .Aggregate(new TagCost(), (r, i) => r + i);

                    results = new List<TagCost>();
                }
            }

            if (results.Any())
            {
                yield return results
                    .Aggregate(new TagCost(), (r, i) => r + i);
            }
        }
    }
}
