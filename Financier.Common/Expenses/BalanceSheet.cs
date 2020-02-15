using System;
using System.Collections.Generic;
using System.Linq;

using Financier.Common.Models;

namespace Financier.Common.Expenses
{
    public class BalanceSheet
    {
        public DateTime InitiatedAt { get; set; }
        public Money InitialCash { get; set; } = Money.Zero;
        public Money InitialDebt { get; set; } = Money.Zero;
        public ICashFlow CashFlow { get; set; }
        public decimal DailyProfit => CashFlow.DailyProfit;
        public Dictionary<DateTime, IList<Money>> CashAdjustments = new Dictionary<DateTime, IList<Money>>();

        private List<Home> homes { get; } = new List<Home>();
        public IReadOnlyList<Home> Homes => homes.AsReadOnly();

        public BalanceSheet(ICashFlow cashFlow, DateTime initiatedAt)
        {
            CashFlow = cashFlow;
            InitiatedAt = initiatedAt;
        }

        public BalanceSheet(Money cash, Money debt, ICashFlow cashFlow, DateTime initiatedAt)
        {
            InitiatedAt = initiatedAt;
            InitialCash = cash;
            InitialDebt = debt;

            CashFlow = cashFlow;
        }

        public void AddHome(Home home)
        {
            if (home.PurchasedAt < InitiatedAt)
            {
                throw new ArgumentOutOfRangeException(nameof(home.PurchasedAt), home.PurchasedAt, $"The home's PurchasedAt cannot be before {InitiatedAt}");
            }

            homes.Add(home);
        }

        public void AddCashAdjustment(DateTime at, Money cash)
        {
            if (CashAdjustments.ContainsKey(at))
            {
                CashAdjustments[at].Add(cash);
            }
            else
            {
                CashAdjustments.Add(at, new List<Money> { cash });
            }
        }

        public decimal GetAssets(IInflation inflation, DateTime at)
        {
            if (at < InitiatedAt)
            {
                throw new ArgumentOutOfRangeException(nameof(at), $"Should be at or later than {InitiatedAt}");
            }

            var result = 0.00M;
            // TODO: add cash adjustments
            result += InitialCash.GetValueAt(inflation, at);
            result += CashFlow.DailyProfit * at.Subtract(InitiatedAt).Days;
            foreach (var home in Homes.Where(item => at > item.PurchasedAt))
            {
                result += home.GetValueAt(at)
                    .Select(item => item.GetValueAt(inflation, at).Value)
                    .Sum();
            }

            return decimal.Round(result, 2);
        }

        public decimal GetLiabilities(IInflation inflation, DateTime at)
        {
            if (at < InitiatedAt)
            {
                throw new ArgumentOutOfRangeException(nameof(at), $"Should be at or later than {InitiatedAt}");
            }

            var result = 0.00M;
            result += InitialDebt.GetValueAt(inflation, at).Value;

            foreach (var home in Homes.Where(item => at > item.PurchasedAt))
            {
                result += home.GetCostAt(at)
                    .Select(item => item.GetValueAt(inflation, at).Value)
                    .Sum();
            }

            return decimal.Round(result, 2);
        }

        public decimal GetNetWorth(IInflation inflation, DateTime at)
        {
            if (at < InitiatedAt)
            {
                throw new ArgumentOutOfRangeException(nameof(at), $"Should be at or later than {InitiatedAt}");
            }

            var result = 0.00M;
            result += GetAssets(inflation, at);
            result -= GetLiabilities(inflation, at);

            return result;
        }

        public decimal GetCashAt(DateTime at)
        {
            if (at < InitiatedAt)
            {
                throw new ArgumentOutOfRangeException(nameof(at), $"Should be at or later than {InitiatedAt}");
            }

            return 0.00M
                + InitialCash.Value
                - InitialDebt
                + CashFlow.DailyProfit * at.Subtract(InitiatedAt).Days;
        }
    }
}
