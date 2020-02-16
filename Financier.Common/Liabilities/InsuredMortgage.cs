using System;
using System.Collections.Generic;

using Financier.Common.Models;

namespace Financier.Common.Liabilities
{
    public class InsuredMortgage : Mortgage
    {
        public const decimal DefaultMaximumInsuranceRate = 0.20M;

        public decimal InsuranceRate { get; }
        public IMortgage BaseMortgage { get; }

        public Money Insurance { get; }
        private Money BaseValue => BaseMortgage.InitialValue;
        public override Money InitialValue => BaseValue + Insurance;
        public override DateTime InitiatedAt => BaseMortgage.InitiatedAt;

        public override int AmortisationPeriodInMonths => BaseMortgage.AmortisationPeriodInMonths;
        public override decimal InterestRate => BaseMortgage.InterestRate;

        public override double PeriodicMonthlyInterestRate => BaseMortgage.PeriodicMonthlyInterestRate;

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
            Calculator = BaseMortgage.Calculator;
            InsuranceRate = downPayment.Value / (downPayment.Value + baseMortgage.InitialValue.Value);
            if (InsuranceRate > maximumInsuranceRate)
            {
                throw new ArgumentOutOfRangeException(nameof(downPayment), $"The down payment cannot exceed more than {maximumInsuranceRate} of the home value {baseMortgage.InitialValue.Value + downPayment.Value}");
            }

            var insuranceValue = BaseValue.Value * (maximumInsuranceRate - InsuranceRate) / 4.0M;
            Insurance = new Money(insuranceValue, BaseValue.At);
        }

        public override IEnumerable<decimal> GetPrincipalOnlyPayments(int year, int month, int day)
        {
            return BaseMortgage.GetPrincipalOnlyPayments(year, month, day);
        }
    }
}
