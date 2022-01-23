using System;

namespace Financier.Common.Liabilities
{
    public class FixedRateMortgage : Mortgage
    {
        public override double PeriodicMonthlyInterestRate => Math.Pow(Math.Pow(((Convert.ToDouble(QuotedInterestRate) / 2) + 1), 2), 1.0 / 12) - 1;

        public FixedRateMortgage(decimal baseValue, decimal interestRate, int amortisationPeriodInMonths, DateTime initiatedAt) : base(baseValue, interestRate, amortisationPeriodInMonths, initiatedAt)
        {
        }
    }
}
