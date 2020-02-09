using System;
using System.Collections.Generic;

namespace Financier.Common.Liabilities
{
    public interface IPrepayable
    {
        void AddPrepayment(DateTime at, decimal amount);
        IEnumerable<ValueTuple<DateTime, decimal>> GetPrepayments(DateTime startAt, DateTime endAt);
    }
}
