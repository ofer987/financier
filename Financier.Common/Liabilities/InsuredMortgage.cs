using System;

using Financier.Common.Models;

namespace Financier.Common.Liabilities
{
    public class InsuredMortgage : IMortgage
    {
        public const decimal DefaultMaximumInsuranceRate = 0.20M;

        public decimal InsuranceRate { get; }
        public IMortgage BaseMortgage { get; }

        public Money BaseValue => BaseMortgage.BaseValue;
        public DateTime InitiatedAt => BaseMortgage.InitiatedAt;

        public int AmortisationPeriodInMonths => BaseMortgage.AmortisationPeriodInMonths;
        public decimal InterestRate => BaseMortgage.InterestRate;
        public decimal QuotedInterestRate => BaseMortgage.QuotedInterestRate;

        public double PeriodicMonthlyInterestRate => BaseMortgage.PeriodicMonthlyInterestRate;
        public decimal PeriodicAnnualInterestRate => BaseMortgage.PeriodicAnnualInterestRate;
        public double EffectiveAnnualInterestRate => BaseMortgage.EffectiveAnnualInterestRate;

        // TODO Figure out the mathematical function for the insurance amount
        public Money Insurance { get; }
        public Money InitialValue => BaseValue + Insurance;

        public static void ValidateInsuranceRate(decimal maximumInsuranceRate)
        {
            if (maximumInsuranceRate < 0.00M || maximumInsuranceRate > 1.0M)
            {
                throw new ArgumentOutOfRangeException(nameof(maximumInsuranceRate), $"Should be between 0.00 per cent and 100.00 per cent");
            }
        }

        public InsuredMortgage(IMortgage baseMortgage, Money downPayment, decimal maximumInsuranceRate = DefaultMaximumInsuranceRate)
        {
            ValidateInsuranceRate(maximumInsuranceRate);

            BaseMortgage = baseMortgage;
            InsuranceRate = downPayment.Value / (downPayment.Value + InitialValue.Value);
            if (InsuranceRate > maximumInsuranceRate)
            {
                throw new ArgumentOutOfRangeException(nameof(downPayment), $"The down payment cannot exceed more than {maximumInsuranceRate} of the home value {InitialValue.Value + downPayment.Value}");
            }

            var insuranceValue = InitialValue.Value * (maximumInsuranceRate - InsuranceRate) / 4.0M;
            Insurance = new Money(insuranceValue, InitialValue.At);
        }
    }
}
