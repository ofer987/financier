using System;

namespace Financier.Common.Expenses
{
    public abstract class BaseCashFlow : ICashFlow
    {
        public virtual decimal DailyProfit => throw new NotImplementedException();

        protected BaseCashFlow()
        {
        }

        public virtual decimal GetCash(DateTime startAt, DateTime endAt)
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
