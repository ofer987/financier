using System;

namespace Financier.Common.Expenses
{
    public class YearlyCashFlow : DurationCashFlow
    {
        public YearlyCashFlow(int year, decimal threshold = DefaultThreshold)
        {
            StartAt = new DateTime(year, 1, 1);
            EndAt = new DateTime(year + 1, 1, 1).AddDays(-1);
            Threshold = threshold;

            Init();
        }
    }
}
