using System;

namespace Financier.Common.Models
{
    public class CompoundMonthlyInflation : IInflation
    {
        private const double DefaultYearlyRate = 0.02d;

        public double MonthlyRate { get; private set; }
        public double YearlyRate { get; private set; }

        public CompoundMonthlyInflation(double yearlyRate = DefaultYearlyRate)
        {
            YearlyRate = yearlyRate;
            MonthlyRate = Math.Pow((1.00d + yearlyRate), 1d / 12);
        }

        public decimal GetValueAt(decimal sourceValue, DateTime sourceAt, DateTime targetAt)
        {
            var totalMonths = 0;

            if (targetAt == sourceAt)
            {
                totalMonths = 0;
            }
            else if (targetAt > sourceAt)
            {
                for (totalMonths = 0; sourceAt.AddMonths(totalMonths) <= targetAt; totalMonths += 1) ;

                totalMonths -= 1;
            }
            else
            {
                for (totalMonths = -1; sourceAt.AddYears(totalMonths) >= targetAt; totalMonths -= 1) ;

                totalMonths += 1;
            }

            var targetValue = Convert.ToDouble(sourceValue) * Math.Pow(MonthlyRate, totalMonths);
            return new Money(Convert.ToDecimal(targetValue), targetAt);
        }
    }
}
