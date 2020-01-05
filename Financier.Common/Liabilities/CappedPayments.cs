using System;
using System.Collections.Generic;

namespace Financier.Common.Liabilities
{
    public class OverPaymentException : Exception
    {
        public decimal MaximumAmount { get; private set; }
        public decimal Amount { get; private set; }

        public override string Message => $"{Amount} exceeded total prepayment amount ({MaximumAmount})";

        public OverPaymentException(decimal maximumAmount, decimal amount)
        {
            Amount = amount;
            MaximumAmount = maximumAmount;
        }
    }

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
            if (annualTotal + amount > MaximumAnnualTotal)
            {
                throw new OverPaymentException(MaximumAnnualTotal, annualTotal + amount);
            }
        }
    }
}
