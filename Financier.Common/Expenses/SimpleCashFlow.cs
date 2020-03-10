using System;
using System.Collections.Generic;
using System.Linq;

namespace Financier.Common.Expenses
{
    public class SimpleCashFlow : ICashFlow
    {
        public DateTime StartAt { get; }
        public DateTime EndAt { get; }
        public decimal DailyProfit { get; }

        public SimpleCashFlow(IEnumerable<decimal> amounts, DateTime startAt, DateTime endAt)
        {
            if (endAt <= startAt)
            {
                throw new ArgumentOutOfRangeException(nameof(startAt), startAt, $"Should be later than {endAt}");
            }

            StartAt = startAt;
            EndAt = endAt;

            var profit = amounts.Sum() / Convert.ToDecimal(EndAt.Subtract(StartAt).TotalDays);
            DailyProfit = decimal.Round(profit, 2);
        }
    }
}
