using System;
using System.Collections.Generic;

namespace Financier.Common.Liabilities
{
    public interface IMonthlyPaymentCalculator
    {
        IEnumerable<MonthlyPayment> GetMonthlyPayments(IMortgage mortgage, DateTime at);
    }
}
