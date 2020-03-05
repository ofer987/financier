using System;

namespace Financier.Common.Liabilities
{
    public class InvalidDownPaymentRateException : ArgumentOutOfRangeException
    {
        public InvalidDownPaymentRateException(decimal rate) : base(nameof(rate), rate, $"Should be between {InsuredMortgage.MinimumInsuranceRate}% and {InsuredMortgage.MaximumInsuranceRate}%")
        {
        }
    }
}
