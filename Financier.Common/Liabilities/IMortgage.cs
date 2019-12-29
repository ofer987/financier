using System.Collections.Generic;

using Financier.Common.Models;

namespace Financier.Common.Liabilities
{
    public interface IMortgage : ILiability
    {
        decimal BaseValue { get; }
        decimal InitialValue { get; }
        double MonthlyPayment { get; }

        int AmortisationPeriodInMonths { get; }
        decimal InterestRate { get; }
        decimal QuotedInterestRate { get; }

        double PeriodicMonthlyInterestRate { get; }
        double PeriodicAnnualInterestRate { get; }
        double EffectiveAnnualInterestRate { get; }

        IEnumerable<decimal> GetMonthlyInterestPayments(int monthAfterInception);
        decimal GetMonthlyInterestPayment(int monthAfterInception);
        decimal GetTotalInterestPayment(int monthAfterInception);

        IEnumerable<decimal> GetMonthlyPrincipalPayments(int monthAfterInception);
        decimal GetTotalPrincipalPayment(int monthAfterInception);
        decimal GetMonthlyPrincipalPayment(int monthAfterInception);

        decimal GetBalance(int monthAfterInception);
    }
}
