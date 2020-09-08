using System;
using System.Collections.Generic;

namespace Financier.Common.Liabilities
{
    public interface IMonthlyPaymentCalculator
    {
        IEnumerable<Payment> GetMonthlyPayments(IMortgage mortgage);
        IEnumerable<Payment> GetMonthlyPayments(IMortgage mortgage, DateTime at);
    }
}
