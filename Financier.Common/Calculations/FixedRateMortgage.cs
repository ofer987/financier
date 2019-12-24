using System;

using Financier.Common.Models;

namespace Financier.Common.Calculations
{
    public class FixedRateMortgage : Liability<Home>
    {
        // public decimal DownPayment { get; }
        public decimal BaseValue { get; }
        public virtual decimal InitialValue => BaseValue;
        public int AmortisationPeriodInMonths { get; }
        public decimal InterestRate { get; }
        public decimal QuotedInterestRate => InterestRate;

        public double PeriodicMonthlyInterestRate => Math.Pow(Math.Pow(((Convert.ToDouble(QuotedInterestRate) / 2) + 1), 2), 1.0/12) - 1;
        public double PeriodicAnnualInterestRate => PeriodicMonthlyInterestRate * 12;
        public double EffectiveAnnualInterestRate => Math.Pow(PeriodicMonthlyInterestRate + 1, 12) - 1;

        public double MonthlyPayment => (Convert.ToDouble(BaseValue) * PeriodicMonthlyInterestRate) / (1 - Math.Pow(1 + PeriodicMonthlyInterestRate, - AmortisationPeriodInMonths));

        public FixedRateMortgage(Home product, decimal baseValue, decimal interestRate, int amortisationPeriodInMonths) : base(product)
        {
            // DownPayment = DownPayment;
            BaseValue = baseValue;
            AmortisationPeriodInMonths = amortisationPeriodInMonths;
            InterestRate = interestRate;
        }

        // // Fixed rate calculations
        // // TODO Create a variable-rate calculation
        // public decimal GetMonthlyPayment()
        // {
        //     var interestRate = PeriodicAnnualInterestRate;
        //
        //     var val = Convert.ToDecimal(Convert.ToDouble(InitialValue) * interestRate / ( 1 - Math.Pow((1 + interestRate), -1 * AmortisationPeriodInMonths)));
        //
        //     return decimal.Round(val, 2);
        // }

        public decimal GetMonthlyInterestPayment(int monthAfterInception)
        {
            if (monthAfterInception < 0)
            {
                throw new Exception($"{nameof(monthAfterInception)} cannot be negative number");
            }

            var monthlyPayment = Convert.ToDecimal(MonthlyPayment);
            var balance = InitialValue;
            var interestRate = PeriodicAnnualInterestRate;

            var interestPayment = 0.00M;
            for (var i = 0; i < monthAfterInception; i += 1)
            {
                interestPayment = Convert.ToDecimal(Convert.ToDouble(balance) * interestRate / 100);

                var principalPayment = monthlyPayment - interestPayment;
                balance -= decimal.Round(Convert.ToDecimal(principalPayment), 2);
            }

            return interestPayment;
        }

        public decimal GetInterestPaymentsBy(int monthAfterInception)
        {
            if (monthAfterInception < 0)
            {
                throw new Exception($"{nameof(monthAfterInception)} cannot be negative number");
            }

            var monthlyPayment = Convert.ToDecimal(MonthlyPayment);
            var balance = InitialValue;
            var interestRate = PeriodicAnnualInterestRate;

            var totalInterestPayments = 0.00M;
            for (var i = 0; i < monthAfterInception; i += 1)
            {
                var interestPayment = Convert.ToDecimal(Convert.ToDouble(balance) * interestRate / 100);
                totalInterestPayments += interestPayment;

                var principalPayment = monthlyPayment - interestPayment;
                balance -= principalPayment;
            }

            return totalInterestPayments;
        }

        public decimal GetPrincipalPaymentsBy(int monthAfterInception)
        {
            if (monthAfterInception < 0)
            {
                throw new Exception($"{nameof(monthAfterInception)} cannot be negative number");
            }

            var monthlyPayment = Convert.ToDecimal(MonthlyPayment);
            var balance = InitialValue;
            var interestRate = PeriodicAnnualInterestRate;

            var totalPrincipalPayments = 0.00M;
            for (var i = 0; i < monthAfterInception; i += 1)
            {
                var interestPayment = Convert.ToDecimal(Convert.ToDouble(balance) * interestRate / 100);
                var principalPayment = monthlyPayment - interestPayment;

                totalPrincipalPayments += principalPayment;
                balance -= principalPayment;
            }

            return totalPrincipalPayments;
        }

        public decimal GetBalance(int monthAfterInception)
        {
            if (monthAfterInception < 0)
            {
                throw new Exception($"{nameof(monthAfterInception)} cannot be negative number");
            }

            var monthlyPayment = Convert.ToDecimal(MonthlyPayment);
            var balance = InitialValue;
            var interestRate = PeriodicAnnualInterestRate;

            for (var i = 0; i < monthAfterInception; i += 1)
            {
                var interestPayment = Convert.ToDecimal(Convert.ToDouble(balance) * interestRate / 12);
                var principalPayment = monthlyPayment - interestPayment;

                balance -= principalPayment;
            }

            return decimal.Round(balance, 2);
        }

        public override decimal CostAt(int monthAfterInception)
        {
            return Convert.ToDecimal(MonthlyPayment);
        }

        public override decimal CostBy(int monthAfterInception)
        {
            return Convert.ToDecimal(MonthlyPayment) * monthAfterInception;
        }
    }
}
