using System;

using Financier.Common.Models;

namespace Financier.Common.Calculations
{
    public class InsuredMortgage : FixedRateMortgage
    {
        public double StartingDownPaymentPercentage => Convert.ToDouble(DownPayment) / Convert.ToDouble(DownPayment + InitialValue);

        // TODO Figure out the mathematical function for the insurance amount
        public decimal Insurance { get; }

        public override decimal InitialValue => BaseValue + Insurance;

        public InsuredMortgage(Home product, decimal downPayment, decimal baseValue, decimal interestRate, int amortisationPeriodInMonths) : base(product, downPayment, baseValue, interestRate, amortisationPeriodInMonths)
        {
        }
    }
}
