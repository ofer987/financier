using System;

using Financier.Common.Models;

namespace Financier.Common.Calculations
{
    public class InsuredMortgage : FixedRateMortgage
    {
        public double StartingDownPaymentPercentage => Convert.ToDouble(Product.DownPayment) / Convert.ToDouble(Product.DownPayment + InitialValue);

        // TODO Figure out the mathematical function for the insurance amount
        public decimal Insurance { get; }

        public override decimal InitialValue => BaseValue + Insurance;

        public InsuredMortgage(Home product, decimal baseValue, decimal interestRate, int amortisationPeriodInMonths) : base(product, baseValue, interestRate, amortisationPeriodInMonths)
        {
        }
    }
}
