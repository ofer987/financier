using System;

using Financier.Common.Models;

namespace Financier.Common.Expenses
{
    public abstract class CashFlow : ICashFlow
    {
        public string AccountName { get; private set; }
        public virtual decimal DailyProfit => throw new NotImplementedException();

        protected CashFlow(string accountName)
        {
            AccountName = accountName;
        }

        public virtual decimal GetCash(IInflation inflation, DateTime startAt, DateTime endAt)
        {
            if (endAt <= startAt)
            {
                throw new ArgumentOutOfRangeException(nameof(startAt), startAt, $"Should be later than {endAt}");
            }

            var result = DailyProfit * endAt.Subtract(startAt).Days;
            return decimal.Round(result, 2);
        }
    }
}
