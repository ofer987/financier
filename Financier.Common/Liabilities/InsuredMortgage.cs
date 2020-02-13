using System;

using Financier.Common.Models;

namespace Financier.Common.Liabilities
{
    public class InsuredMortgage : FixedRateMortgage
    {
        [Obsolete]
        public double StartingDownPaymentPercentage => Convert.ToDouble(DownPayment) / Convert.ToDouble(DownPayment + InitialValue);

        // TODO Figure out the mathematical function for the insurance amount
        public Money Insurance { get; }
        public decimal DownPayment { get; }

        public override Money InitialValue => BaseValue + Insurance;

        [Obsolete]
        public InsuredMortgage(Money baseValue, decimal interestRate, int amortisationPeriodInMonths) : base(baseValue, interestRate, amortisationPeriodInMonths, DateTime.UtcNow)
        {
        }
    }
}
