using System;
using System.Collections.Generic;
using System.Linq;

using Financier.Common.Expenses.Models;

// TODO: Remove hardcoded filter tag names and replace with functions

namespace Financier.Common.Expenses
{
    public class CashFlow : ICashFlow
    {
        public decimal Threshold { get; protected set; }
        protected const decimal DefaultThreshold = 0.05M;

        public DateTime StartAt { get; protected set; }
        public DateTime EndAt { get; protected set; }

        public IReadOnlyList<ItemListing> CreditListings { get; private set; } = Enumerable.Empty<ItemListing>().ToList();
        public IReadOnlyList<ItemListing> DebitListings { get; private set; } = Enumerable.Empty<ItemListing>().ToList();

        public IReadOnlyList<ItemListing> CombinedCreditListings { get; private set; } = Enumerable.Empty<ItemListing>().ToList();
        public IReadOnlyList<ItemListing> CombinedDebitListings { get; private set; } = Enumerable.Empty<ItemListing>().ToList();

        public decimal CreditAmountTotal { get; private set; } = 0.00M;
        public decimal DebitAmountTotal { get; private set; } = 0.00M;
        public decimal ProfitAmountTotal => CreditAmountTotal - DebitAmountTotal;

        public decimal DailyProfit => decimal.Round(ProfitAmountTotal / EndAt.Subtract(StartAt).Days, 2);

        public CashFlow(DateTime startAt, DateTime endAt, decimal threshold = DefaultThreshold)
        {
            StartAt = startAt;
            EndAt = endAt;
            Threshold = threshold;

            Init();
        }

        protected CashFlow()
        {
        }

        protected void Init()
        {
            SetCredits();
            SetDebits();
        }

        private void SetCredits()
        {
            CreditListings = CashFlowHelper.GetItemListings(StartAt, EndAt, ItemTypes.Credit);
            CombinedCreditListings = CashFlowHelper.CombineItemListings(CreditListings, Threshold);
            CreditAmountTotal = CreditListings
                .Select(cost => cost.Amount)
                .Aggregate(0.00M, (r, i) => r + i);
        }

        private void SetDebits()
        {
            DebitListings = CashFlowHelper.GetItemListings(StartAt, EndAt, ItemTypes.Debit);
            CombinedDebitListings = CashFlowHelper.CombineItemListings(CreditListings, Threshold);
            DebitAmountTotal = DebitListings
                .Select(cost => cost.Amount)
                .Aggregate(0.00M, (r, i) => r + i);
        }
    }
}
