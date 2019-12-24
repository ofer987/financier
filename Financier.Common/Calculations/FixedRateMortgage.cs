using System;

using Financier.Common.Models;

namespace Financier.Common.Calculations
{
    public class FixedRateMortgage : Mortgage
    {
        public override double PeriodicMonthlyInterestRate => Math.Pow(Math.Pow(((Convert.ToDouble(QuotedInterestRate) / 2) + 1), 2), 1.0/12) - 1;

        public FixedRateMortgage(Home product, decimal baseValue, decimal interestRate, int amortisationPeriodInMonths) : base(product, baseValue, interestRate, amortisationPeriodInMonths)
        {
        }
    }
}