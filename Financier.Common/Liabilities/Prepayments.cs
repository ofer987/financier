using System;
using System.Collections.Generic;
using System.Linq;

namespace Financier.Common.Liabilities
{
    public class Prepayments
    {
        public decimal Total { get; }
        public decimal MaximumAllowedAnnualTotal { get; }

        private IDictionary<DateTime, decimal> amounts = new Dictionary<DateTime, decimal>();

        public Prepayments(decimal total, decimal maximumAllowedAnnualPercentage = 0.10M)
        {
            if (total <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(total), "Should be greater than 0");
            }
            if (maximumAllowedAnnualPercentage <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maximumAllowedAnnualPercentage), "Should be greater than 0");
            }

            Total = total;
            MaximumAllowedAnnualTotal = decimal.Round(Total * maximumAllowedAnnualPercentage, 2);
        }

        public void Add(DateTime at, decimal amount)
        {
            var annualTotal = GetAnnualTotal(at.Year);

            if (annualTotal >= MaximumAllowedAnnualTotal)
            {
                throw new Exception($"Exceeded total prepayment amount ({MaximumAllowedAnnualTotal})");
            }

            amounts.Add(at, amount);
        }

        public decimal GetAnnualTotal(int year)
        {
            var beginningOfYear = new DateTime(year, 1, 1);
            var beginningOfNextYear = new DateTime(year + 1, 1, 1);

            return amounts
                .Where(prepayment => prepayment.Key >= beginningOfYear)
                .Where(prepayment => prepayment.Key < beginningOfNextYear)
                .Select(prepayment => prepayment.Value)
                .Sum();
        }

        public IEnumerable<ValueTuple<DateTime, decimal>> GetAll(DateTime startAt, DateTime endAt)
        {
            if (endAt <= startAt)
            {
                throw new ArgumentOutOfRangeException(nameof(endAt), $"Should be after {nameof(startAt)}");
            }

            return amounts
                .Where(prepayment => prepayment.Key >= startAt)
                .Where(prepayment => prepayment.Key < endAt)
                .Select(prepayment => ValueTuple.Create(prepayment.Key, prepayment.Value));
        }

        public decimal Get(int year, int month)
        {
            var beginningOfMonth = new DateTime(year, month, 1);
            var beginningOfNextMonth = new DateTime(year, month, 1).AddMonths(1);

            return amounts
                .Where(prepayment => prepayment.Key >= beginningOfMonth)
                .Where(prepayment => prepayment.Key < beginningOfNextMonth)
                .Select(prepayment => prepayment.Value)
                .Sum();
        }

    }
}
