using System;

namespace Financier.Common.Expenses
{
    public class MonthlyCashFlow : CashFlow
    {
        public DateTime At => StartAt;

        public MonthlyCashFlow(int year, int month, decimal threshold = DefaultThreshold) : base(threshold)
        {
            StartAt = new DateTime(year, month, 1);
            EndAt = new DateTime(year, month, 1).AddMonths(1);

            Init();
        }
    }
}
