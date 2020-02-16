using System;
using System.Collections.Generic;

using Financier.Common.Models;

namespace Financier.Common.Liabilities
{
    public interface IMortgage
    {
        Money BaseValue { get; }
        Money InitialValue { get; }
        double MonthlyPayment { get; }

        DateTime InitiatedAt { get; }
        int AmortisationPeriodInMonths { get; }
        decimal InterestRate { get; }
        decimal QuotedInterestRate { get; }

        double PeriodicMonthlyInterestRate { get; }
        decimal PeriodicAnnualInterestRate { get; }
        double EffectiveAnnualInterestRate { get; }

        Money GetBalance(DateTime at);
        IEnumerable<MonthlyPayment> GetMonthlyPayments();
        IEnumerable<MonthlyPayment> GetMonthlyPayments(DateTime endAt);
        IEnumerable<decimal> GetPrincipalOnlyPayments(int year, int month, int day);

        bool IsMonthlyPayment(DateTime at);
    }
}
