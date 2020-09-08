using System;

using Financier.Common.Models;

namespace Financier.Common.Expenses
{
    public interface ICashFlow
    {
        decimal DailyProfit { get; }
        decimal GetCash(IInflation inflation, DateTime startAt, DateTime endAt);
    }
}
