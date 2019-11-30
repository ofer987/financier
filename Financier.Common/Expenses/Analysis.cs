using System;
using System.Collections.Generic;
using System.Linq;

using Financier.Common.Expenses.Models;

// TODO: Remove hardcoded filter tag names and replace with functions

namespace Financier.Common.Expenses
{
    public class Analysis
    {
        public decimal Threshold { get; private set; }
        protected const decimal DefaultThreshold = 0.05M;

        public DateTime StartAt { get; protected set; }
        public DateTime EndAt { get; protected set; }

        public IReadOnlyList<ItemListing> AssetListings { get; private set; } = Enumerable.Empty<ItemListing>().ToList();
        public IReadOnlyList<ItemListing> ExpenseListings { get; private set; } = Enumerable.Empty<ItemListing>().ToList();

        public IReadOnlyList<ItemListing> CombinedAssetListings { get; private set; } = Enumerable.Empty<ItemListing>().ToList();
        public IReadOnlyList<ItemListing> CombinedExpenseListings { get; private set; } = Enumerable.Empty<ItemListing>().ToList();

        public decimal AssetAmountTotal { get; private set; } = 0.00M;
        public decimal ExpenseAmountTotal { get; private set; } = 0.00M;
        public decimal ProfitAmountTotal => AssetAmountTotal - ExpenseAmountTotal;


        public Analysis(DateTime startAt, DateTime endAt, decimal threshold = DefaultThreshold) : this(threshold)
        {
            StartAt = startAt;
            EndAt = endAt;

            Init();
        }

        public Analysis(decimal threshold = DefaultThreshold)
        {
            Threshold = threshold;

            Init();
        }

        protected void Init()
        {
            SetAssets();
            SetExpenses();
        }

        private void SetAssets()
        {
            AssetListings = AnalysisHelper.GetItemListings(StartAt, EndAt, ItemTypes.Credit);
            CombinedAssetListings = AnalysisHelper.CombineItemListings(AssetListings, Threshold);
            AssetAmountTotal = AssetListings
                .Select(cost => cost.Amount)
                .Aggregate(0.00M, (r, i) => r + i);
        }

        private void SetExpenses()
        {
            ExpenseListings = AnalysisHelper.GetItemListings(StartAt, EndAt, ItemTypes.Debit);
            CombinedExpenseListings = AnalysisHelper.CombineItemListings(AssetListings, Threshold);
            ExpenseAmountTotal = ExpenseListings
                .Select(cost => cost.Amount)
                .Aggregate(0.00M, (r, i) => r + i);
        }
    }
}
