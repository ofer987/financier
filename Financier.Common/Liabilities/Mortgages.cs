using System;

using Financier.Common.Models;

namespace Financier.Common.Liabilities
{
    public class Mortgages
    {
        public static IMortgage GetFixedRateMortgage(Money baseValue, decimal interestRate, int amortisationPeriodInMonths, DateTime initiatedAt, Money downPayment)
        {
            var result = new FixedRateMortgage(
                baseValue,
                interestRate,
                amortisationPeriodInMonths,
                initiatedAt
            );

            var downPaymentRate = downPayment.Value / (baseValue.Value + downPayment.Value);
            if (InsuredMortgage.IsInsured(downPaymentRate))
            {
                return new InsuredMortgage(result, downPayment);
            }

            return result;
        }

        public static IMortgage GetVariableRateMortgage(Money baseValue, decimal interestRate, int amortisationPeriodInMonths, DateTime initiatedAt, Money downPayment, decimal maximumInsuranceRate = InsuredMortgage.MaximumInsuranceRate)
        {
            var result = new VariableRateMortgage(
                baseValue,
                interestRate,
                amortisationPeriodInMonths,
                initiatedAt
            );

            var downPaymentRate = downPayment.Value / (baseValue.Value + downPayment.Value);
            if (InsuredMortgage.IsInsured(downPaymentRate))
            {
                return new InsuredMortgage(result, downPayment);
            }

            return result;
        }
    }
}
