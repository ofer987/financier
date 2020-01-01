using System;

using Financier.Common.Models;

namespace Financier.Common.Liabilities
{
    public class FixedRateMortgage : Mortgage
    {
        public override double PeriodicMonthlyInterestRate => Math.Pow(Math.Pow(((Convert.ToDouble(QuotedInterestRate) / 2) + 1), 2), 1.0/12) - 1;

        public FixedRateMortgage(Home product, DateTime initiatedAt, decimal baseValue, decimal interestRate, int amortisationPeriodInMonths) : base(product, initiatedAt, baseValue, interestRate, amortisationPeriodInMonths)
        {
        }
    }
}
