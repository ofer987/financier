using System;

namespace Financier.Common.Liabilities
{
    public class InvalidDownPaymentRateException : ArgumentOutOfRangeException
    {
        public InvalidDownPaymentRateException(decimal rate) : base(nameof(rate), rate, $"Should be between {InsuredMortgage.MinimumInsuranceRate * 100}% and {InsuredMortgage.MaximumInsuranceRate * 100}%")
        {
        }
    }
}
