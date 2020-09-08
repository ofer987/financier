using System;
using System.Collections.Generic;
using System.Linq;

namespace Financier.Common.Liabilities
{
    public class PrepayableMortgage : Mortgage, IPrepayable
    {
        public IMortgage BaseMortgage { get; }
        public CappedPayments Prepayments { get; }

        private decimal BaseValue => BaseMortgage.InitialValue;
        public override decimal InitialValue => BaseValue;
        public override DateTime InitiatedAt => BaseMortgage.InitiatedAt;
        public override Guid Id => BaseMortgage.Id;
        public override string Name => BaseMortgage.Name;

        public override int AmortisationPeriodInMonths => BaseMortgage.AmortisationPeriodInMonths;
        public override decimal InterestRate => BaseMortgage.InterestRate;
        public override double PeriodicMonthlyInterestRate => BaseMortgage.PeriodicMonthlyInterestRate;

        public decimal MaximumAllowedPrepaymentTotal => Prepayments.MaximumAnnualTotal;

        public PrepayableMortgage(IMortgage baseMortgage, IMonthlyPaymentCalculator calculator, decimal maximumAllowedPrepaymentPercentage = 0.10M)
        {
            BaseMortgage = baseMortgage;
            Calculator = calculator;
            Prepayments = new CappedPayments(InitialValue * maximumAllowedPrepaymentPercentage);
        }

        public PrepayableMortgage(IMortgage baseMortgage, decimal maximumAllowedPrepaymentPercentage = 0.10M)
        {
            BaseMortgage = baseMortgage;
            Calculator = new MonthlyPaymentCalculator();
            Prepayments = new CappedPayments(InitialValue * maximumAllowedPrepaymentPercentage);
        }

        public override IEnumerable<decimal> GetPrincipalOnlyPayments(int year, int month, int day)
        {
            var startAt = new DateTime(year, month, day);
            var endAt = startAt.AddDays(1);

            return GetPrepayments(startAt, endAt)
                .Select(payment => payment.Item2);
        }

        public void AddPrepayment(DateTime at, decimal amount)
        {
            Prepayments.Add(at, amount);
        }

        public IEnumerable<ValueTuple<DateTime, decimal>> GetPrepayments(DateTime startAt, DateTime endAt)
        {
            return Prepayments.GetRange(startAt, endAt);
        }
    }
}
