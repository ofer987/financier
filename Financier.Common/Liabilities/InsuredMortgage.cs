using System;
using System.Collections.Generic;

using Financier.Common.Models;

namespace Financier.Common.Liabilities
{
    public class InsuredMortgage : Mortgage
    {
        public const decimal MinimumInsuranceRate = 0.05M;
        public const decimal MaximumInsuranceRate = 0.20M;

        public override Guid Id => BaseMortgage.Id;

        public decimal InsuranceRate { get; }
        public IMortgage BaseMortgage { get; }

        public Money Insurance { get; }
        private Money BaseValue => BaseMortgage.InitialValue;
        public override Money InitialValue => BaseValue + Insurance;
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

        public InsuredMortgage(IMortgage baseMortgage, Money downPayment) : this(baseMortgage, downPayment.Value)
        {
        }

        public InsuredMortgage(IMortgage baseMortgage, decimal downPayment)
        {
            var rate = downPayment / (downPayment + baseMortgage.InitialValue.Value);
            ValidateInsuranceRate(rate);

            BaseMortgage = baseMortgage;
            Calculator = BaseMortgage.Calculator;
            InsuranceRate = downPayment / (downPayment + baseMortgage.InitialValue.Value);

            Insurance = new Money(
                GetInsurance(baseMortgage.InitialValue, downPayment),
                InitiatedAt
            );
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
                return BaseValue.Value * (MaximumInsuranceRate - rate) * 0.30M;
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
