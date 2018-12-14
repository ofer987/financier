using System;

using Financier.Common.Models;

namespace Financier.Common.Calculations
{
    public class InsuredMortgage : Mortgage
    {
        public double StartingDownPaymentPercentage => Convert.ToDouble(DownPayment) / Convert.ToDouble(DownPayment + Value);

        // TODO Figure out the mathematical function for the insurance amount
        public decimal Insurance { get; }

        public override decimal Value => BaseValue + Insurance;

        public InsuredMortgage(decimal downPayment, decimal baseValue, int amortisationPeriodInMonths, decimal interestRate) : base(downPayment, baseValue, amortisationPeriodInMonths, interestRate)
        {
        }
    }
}
