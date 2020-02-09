using System;
using System.Collections.Generic;
using System.Linq;

namespace Financier.Common.Liabilities
{
    public class Payments
    {
        private IDictionary<DateTime, decimal> amounts = new Dictionary<DateTime, decimal>();

        public void Add(DateTime at, decimal amount)
        {
            Validate(at, amount);

            amounts.Add(at, amount);
        }

        public IEnumerable<ValueTuple<DateTime, decimal>> GetAll()
        {
            return amounts
                .Select(prepayment => ValueTuple.Create(prepayment.Key, prepayment.Value));
        }

        public IEnumerable<ValueTuple<DateTime, decimal>> GetRange(DateTime startAt, DateTime endAt)
        {
            if (endAt < startAt)
            {
                throw new ArgumentOutOfRangeException(nameof(endAt), $"Should be after {nameof(startAt)}");
            }

            return amounts
                .Where(prepayment => prepayment.Key >= startAt)
                .Where(prepayment => prepayment.Key < endAt)
                .Select(prepayment => ValueTuple.Create(prepayment.Key, prepayment.Value));
        }

        public decimal GetAnnualTotal(int year)
        {
            var beginningOfYear = new DateTime(year, 1, 1);
            var beginningOfNextYear = new DateTime(year + 1, 1, 1);

            return GetRange(beginningOfYear, beginningOfNextYear)
                .Select(amount => amount.Item2)
                .Sum();
        }

        public decimal GetMonthlyTotal(int year, int month)
        {
            var beginningOfMonth = new DateTime(year, month, 1);
            var beginningOfNextMonth = new DateTime(year, month, 1).AddMonths(1);

            return GetRange(beginningOfMonth, beginningOfNextMonth)
                .Select(amount => amount.Item2)
                .Sum();
        }

        public decimal GetDailyTotal(int year, int month, int day)
        {
            var beginningOfDay = new DateTime(year, month, 1);
            var beginningOfNextDay = new DateTime(year, month, 1).AddMonths(1);

            return GetRange(beginningOfDay, beginningOfNextDay)
                .Select(amount => amount.Item2)
                .Sum();
        }

        protected virtual void Validate(DateTime at, decimal amount)
        {
        }
    }
}
