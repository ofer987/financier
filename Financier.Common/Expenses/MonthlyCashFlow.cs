using System;

namespace Financier.Common.Expenses
{
    public class MonthlyCashFlow : DurationCashFlow
    {
        public MonthlyCashFlow(string accountName, int year, int month, decimal threshold = DefaultThreshold) : base(accountName)
        {
            StartAt = new DateTime(year, month, 1);
            EndAt = new DateTime(year, month, 1).AddMonths(1);
            Threshold = threshold;

            Init();
        }
    }
}
