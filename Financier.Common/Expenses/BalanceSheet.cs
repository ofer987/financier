using System;
using System.Collections.Generic;
using System.Linq;

using Financier.Common.Extensions;
using Financier.Common.Models;
using Financier.Common.Expenses.Actions;

namespace Financier.Common.Expenses
{
    public class BalanceSheet
    {
        public DateTime At { get; }
        public Activity ProductHistory { get; }

        public BalanceSheet(Activity productHistory, DateTime at)
        {
            ProductHistory = productHistory;
            At = at;
        }

        public CashFlowStatement GetCashFlowStatement(DateTime startAt, DateTime endAt)
        {
            return new CashFlowStatement(ProductHistory, startAt, endAt);
        }

        // public IEnumerable<Product> GetOwnedProducts(DateTime at)
        // {
        //     return ProductHistory.GetOwnedProducts(at);
        // }
        //
        // public decimal GetAssets(IInflation inflation, DateTime at)
        // {
        //     if (at < InitiatedAt)
        //     {
        //         throw new ArgumentOutOfRangeException(nameof(at), $"Should be at or later than {InitiatedAt}");
        //     }
        //
        //     var result = 0.00M;
        //     // TODO: add cash adjustments
        //     result += InitialCash.GetValueAt(inflation, at);
        //     result += CashFlow.DailyProfit * at.Subtract(InitiatedAt).Days;
        //     result += GetValueOfOwnedProducts(inflation, at);
        //
        //     return decimal.Round(result, 2);
        // }
        //
        // public decimal GetValueOfOwnedProducts(IInflation inflation, DateTime at)
        // {
        //     return GetOwnedProducts(at)
        //         .SelectMany(product => product.GetValueAt(at))
        //         .InflatedValue(inflation, at);
        // }
        //
        // public decimal GetCostOfOwnedProducts(IInflation inflation, DateTime at)
        // {
        //     return GetOwnedProducts(at)
        //         .SelectMany(product => product.GetCostAt(at))
        //         .InflatedValue(inflation, at);
        // }
        //
        // public decimal GetLiabilities(IInflation inflation, DateTime at)
        // {
        //     if (at < InitiatedAt)
        //     {
        //         throw new ArgumentOutOfRangeException(nameof(at), $"Should be at or later than {InitiatedAt}");
        //     }
        //
        //     var result = 0.00M;
        //     result += InitialDebt.GetValueAt(inflation, at).Value;
        //     result += GetCostOfOwnedProducts(inflation, at);
        //
        //     return decimal.Round(result, 2);
        // }
        //
        // public decimal GetNetWorth(IInflation inflation, DateTime at)
        // {
        //     if (at < InitiatedAt)
        //     {
        //         throw new ArgumentOutOfRangeException(nameof(at), $"Should be at or later than {InitiatedAt}");
        //     }
        //
        //     var result = 0.00M;
        //     result += GetAssets(inflation, at);
        //     result -= GetLiabilities(inflation, at);
        //
        //     return result;
        // }
        //
        // public decimal GetCashAt(DateTime at)
        // {
        //     if (at < InitiatedAt)
        //     {
        //         throw new ArgumentOutOfRangeException(nameof(at), $"Should be at or later than {InitiatedAt}");
        //     }
        //
        //     return 0.00M
        //         + InitialCash.Value
        //         - InitialDebt
        //         + CashFlow.DailyProfit * at.Subtract(InitiatedAt).Days;
        // }
    }
}
