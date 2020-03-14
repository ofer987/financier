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

            return ConvertToInsuredMortgageIfInsurable(result, downPayment);
        }

        public static IMortgage GetVariableRateMortgage(Money baseValue, decimal interestRate, int amortisationPeriodInMonths, DateTime initiatedAt, Money downPayment, decimal maximumInsuranceRate = InsuredMortgage.MaximumInsuranceRate)
        {
            var result = new VariableRateMortgage(
                baseValue,
                interestRate,
                amortisationPeriodInMonths,
                initiatedAt
            );

            return ConvertToInsuredMortgageIfInsurable(result, downPayment);
        }

        private static IMortgage ConvertToInsuredMortgageIfInsurable(IMortgage mortgage, decimal downPayment)
        {
            // TODO: Maybe use Money instead of decimal?
            var downPaymentRate = downPayment / (mortgage.InitialValue.Value + downPayment);
            var insuranceTypeRequired = InsuredMortgage.IsInsurable(downPaymentRate);

            switch (insuranceTypeRequired)
            {
                case InsuranceTypes.DownPaymentLow:
                    throw new InvalidDownPaymentRateException(downPaymentRate);
                case InsuranceTypes.Insurable:
                    return new InsuredMortgage(mortgage, downPayment);
                case InsuranceTypes.NotRequired:
                default:
                    return mortgage;
            }
        }
    }
}
