using System;
using System.Collections.Generic;

using Financier.Common.Models;

namespace Financier.Common.Liabilities
{
    public interface IMortgage : IProduct
    {
        IMonthlyPaymentCalculator Calculator { get; }

        decimal InitialValue { get; }
        double MonthlyPayment { get; }

        DateTime InitiatedAt { get; }
        int AmortisationPeriodInMonths { get; }
        decimal InterestRate { get; }
        decimal QuotedInterestRate { get; }

        double PeriodicMonthlyInterestRate { get; }
        decimal PeriodicAnnualInterestRate { get; }
        double EffectiveAnnualInterestRate { get; }

        decimal GetBalance(DateTime at);
        IEnumerable<Payment> GetMonthlyPayments();
        IEnumerable<Payment> GetMonthlyPayments(DateTime endAt);
        IEnumerable<decimal> GetPrincipalOnlyPayments(int year, int month, int day);

        bool IsMonthlyPayment(DateTime at);
    }
}
