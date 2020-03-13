using System;

namespace Financier.Common.Expenses
{
    public interface ICashFlow
    {
        decimal DailyProfit { get; }
        decimal GetCash(DateTime startAt, DateTime endAt);
    }
}
