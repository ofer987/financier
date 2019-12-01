using System;

namespace Financier.Common.Expenses
{
    public class MonthlyCashFlow : CashFlow
    {
        public DateTime At => StartAt;

        public MonthlyCashFlow(int year, int month, decimal threshold = DefaultThreshold) : base(threshold)
        {
            StartAt = new DateTime(year, month, 1);

            if (month == 12)
            {
                EndAt = new DateTime(year + 1, 1, 1);
            }
            else
            {
                EndAt = new DateTime(year, month + 1, 1);
            }

            Init();
        }
    }
}
