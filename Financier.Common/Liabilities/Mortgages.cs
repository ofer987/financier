using System;

namespace Financier.Common.Liabilities
{
    public class Mortgages
    {
        public static IMortgage GetFixedRateMortgage(decimal baseValue, decimal interestRate, int amortisationPeriodInMonths, DateTime initiatedAt, decimal downPayment)
        {
            var result = new FixedRateMortgage(
                baseValue,
                interestRate,
                amortisationPeriodInMonths,
                initiatedAt
            );

            return ConvertToInsuredMortgageIfInsurable(result, downPayment);
        }

        public static IMortgage GetVariableRateMortgage(decimal baseValue, decimal interestRate, int amortisationPeriodInMonths, DateTime initiatedAt, decimal downPayment, decimal maximumInsuranceRate = InsuredMortgage.MaximumInsuranceRate)
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
            var downPaymentRate = downPayment / (mortgage.InitialValue + downPayment);
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
