using System;
using System.Collections.Generic;
using System.Linq;

using Financier.Common.Models;

namespace Financier.Common.Expenses
{
    public class BalanceSheet
    {
        public DateTime InitiatedAt { get; }
        public Money InitialCash { get; }
        public Money InitialDebt { get; }
        public ICashFlow CashFlow { get; }
        public decimal DailyProfit => CashFlow.DailyProfit;

        private List<Home> homes { get; } = new List<Home>();
        public IReadOnlyList<Home> Homes => homes.AsReadOnly();

        public BalanceSheet(Money cash, Money debt, ICashFlow cashFlow, DateTime initiatedAt)
        {
            InitiatedAt = initiatedAt;
            InitialCash = cash;
            InitialDebt = debt;

            CashFlow = cashFlow;
        }

        public void AddHome(Home home)
        {
            homes.Add(home);
        }

        public decimal GetAssets(IInflation inflation, DateTime at)
        {
            if (at < InitiatedAt)
            {
                throw new ArgumentOutOfRangeException(nameof(at), $"Should be at or later than {InitiatedAt}");
            }

            var result = 0.00M;
            result += InitialCash.GetValueAt(inflation, at);
            result += CashFlow.DailyProfit * at.Subtract(InitiatedAt).Days;
            foreach (var home in Homes.Where(item => at > item.PurchasedAt))
            {
                result += home.DownPayment.GetValueAt(inflation, at);
                result += home.Financing.GetMonthlyPayments(at)
                    .Select(payment => payment.Principal.GetValueAt(inflation, at).Value)
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
            result += InitialDebt.GetValueAt(inflation, at);

            foreach (var home in Homes.Where(item => at > item.PurchasedAt))
            {
                result += home.Financing.GetBalance(at);
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

        public virtual decimal GetBalance(int months)
        {
            throw new NotImplementedException();
        }
    }
}
