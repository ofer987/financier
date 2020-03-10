using System;

namespace Financier.Common.Liabilities
{
    public class InvalidDownPaymentRateException : ArgumentOutOfRangeException
    {
        public InvalidDownPaymentRateException(decimal rate) : base(nameof(rate), 100.00M * rate, $"Should be between {InsuredMortgage.MinimumInsuranceRate * 100.00M}% and {InsuredMortgage.MaximumInsuranceRate * 100.00M}%")
        {
        }
    }
}
