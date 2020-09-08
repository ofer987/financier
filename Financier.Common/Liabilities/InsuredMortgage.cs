using System;
using System.Collections.Generic;

namespace Financier.Common.Liabilities
{
    public class InsuredMortgage : Mortgage
    {
        public const decimal MinimumInsuranceRate = 0.05M;
        public const decimal MaximumInsuranceRate = 0.20M;

        public override Guid Id => BaseMortgage.Id;

        public decimal InsuranceRate { get; }
        public IMortgage BaseMortgage { get; }

        public decimal Insurance { get; }
        private decimal BaseValue => BaseMortgage.InitialValue;
        public override decimal InitialValue => BaseValue + Insurance;
        public override DateTime InitiatedAt => BaseMortgage.InitiatedAt;

        public override int AmortisationPeriodInMonths => BaseMortgage.AmortisationPeriodInMonths;
        public override decimal InterestRate => BaseMortgage.InterestRate;

        public override double PeriodicMonthlyInterestRate => BaseMortgage.PeriodicMonthlyInterestRate;

        public static InsuranceTypes IsInsurable(decimal rate)
        {
            if (rate < MinimumInsuranceRate)
            {
                return InsuranceTypes.DownPaymentLow;
            }
            else if (rate >= MinimumInsuranceRate && rate < MaximumInsuranceRate)
            {
                return InsuranceTypes.Insurable;
            }

            return InsuranceTypes.NotRequired;
        }

        public static void ValidateInsuranceRate(decimal rate)
        {
            if (IsInsurable(rate) != InsuranceTypes.Insurable)
            {
                throw new InvalidDownPaymentRateException(rate);
            }
        }

        public InsuredMortgage(IMortgage baseMortgage, decimal downPayment)
        {
            var rate = downPayment / (downPayment + baseMortgage.InitialValue);
            ValidateInsuranceRate(rate);

            BaseMortgage = baseMortgage;
            Calculator = BaseMortgage.Calculator;
            InsuranceRate = downPayment / (downPayment + baseMortgage.InitialValue);

            Insurance = GetInsurance(baseMortgage.InitialValue, downPayment);
        }

        public override IEnumerable<decimal> GetPrincipalOnlyPayments(int year, int month, int day)
        {
            return BaseMortgage.GetPrincipalOnlyPayments(year, month, day);
        }

        private decimal GetInsurance(decimal mortgageAmount, decimal downPayment)
        {
            var total = downPayment + mortgageAmount;
            var rate = downPayment / total;
            if (rate >= MinimumInsuranceRate && rate < 0.10M)
            {
                return BaseValue * (MaximumInsuranceRate - rate) * 0.30M;
            }
            else if (rate >= 0.10M && rate < 0.15M)
            {
                return total * 135.00M / 4500.00M;
            }
            else
            {
                // rate should be MinimumInsuranceRate <= x < MaximumInsuranceRate
                return total * 115.00M / 4500.00M;
            }
        }
    }
}
