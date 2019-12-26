using System.Collections.Generic;

using Financier.Common.Models;

namespace Financier.Common.Liabilities
{
    public interface IMortgage : ILiability
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
