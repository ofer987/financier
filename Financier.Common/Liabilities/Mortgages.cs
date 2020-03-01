using System;

using Financier.Common.Models;

namespace Financier.Common.Liabilities
{
    public class Mortgages
    {
        public static IMortgage GetFixedRateMortgage(Money baseValue, decimal interestRate, int amortisationPeriodInMonths, DateTime initiatedAt, Money downPayment, decimal maximumInsuranceRate = InsuredMortgage.DefaultMaximumInsuranceRate)
        {
            InsuredMortgage.ValidateInsuranceRate(maximumInsuranceRate);

            var result = new FixedRateMortgage(
                baseValue,
                interestRate,
                amortisationPeriodInMonths,
                initiatedAt
            );

            if ((downPayment.Value / (baseValue.Value + downPayment.Value)) < maximumInsuranceRate)
            {
                return new InsuredMortgage(result, downPayment, maximumInsuranceRate);
            }

            return result;
        }

        public static IMortgage GetVariableRateMortgage(Money baseValue, decimal interestRate, int amortisationPeriodInMonths, DateTime initiatedAt, Money downPayment, decimal maximumInsuranceRate = InsuredMortgage.DefaultMaximumInsuranceRate)
        {
            InsuredMortgage.ValidateInsuranceRate(maximumInsuranceRate);

            var result = new VariableRateMortgage(
                baseValue,
                interestRate,
                amortisationPeriodInMonths,
                initiatedAt
            );

            if ((downPayment.Value / (baseValue.Value + downPayment.Value)) < maximumInsuranceRate)
            {
                return new InsuredMortgage(result, downPayment, maximumInsuranceRate);
            }

            return result;
        }
    }
}
