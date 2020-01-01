using System;
using System.Collections.Generic;

namespace Financier.Common.Liabilities
{
    public class CappedPayments : Payments
    {
        public decimal MaximumAnnualTotal { get; }

        private IDictionary<DateTime, decimal> amounts = new Dictionary<DateTime, decimal>();

        public CappedPayments(decimal maximumAnnualTotal) : base()
        {
            if (maximumAnnualTotal <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maximumAnnualTotal), "Should be greater than 0");
            }

            MaximumAnnualTotal = maximumAnnualTotal;
        }

        protected override void Validate(DateTime at, decimal amount)
        {
            var annualTotal = GetAnnualTotal(at.Year);
            if (annualTotal + amount >= MaximumAnnualTotal)
            {
                throw new Exception($"Exceeded total prepayment amount ({MaximumAnnualTotal})");
            }
        }
    }
}
