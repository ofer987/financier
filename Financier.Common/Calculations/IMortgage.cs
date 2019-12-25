using System.Collections.Generic;

namespace Financier.Common.Calculations
{
    public interface IMortgage
    {
        double MonthlyPayment { get; }

        IEnumerable<decimal> GetMonthlyInterestPayments(int monthAfterInception);

        decimal GetMonthlyInterestPayment(int monthAfterInception);

        decimal GetTotalInterestPayment(int monthAfterInception);

        IEnumerable<decimal> GetMonthlyPrincipalPayments(int monthAfterInception);

        decimal GetTotalPrincipalPayment(int monthAfterInception);

        decimal GetMonthlyPrincipalPayment(int monthAfterInception);

        decimal GetBalance(int monthAfterInception);
    }
}
