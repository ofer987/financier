using System;

namespace Financier.Common.Models
{
    public class CompoundYearlyInflation : IInflation
    {
        public const decimal YearlyInflation = 0.02M;

        public Money GetValueAt(Money source, DateTime targetAt)
        {
            int totalYears;
            if (targetAt > source.At)
            {
                totalYears = Convert.ToInt32(
                    Math.Floor((targetAt - source.At).TotalDays / 365)
                );
            }
            else
            {
                totalYears = Convert.ToInt32(
                    Math.Floor((source.At - targetAt).TotalDays / 365)
                );
                totalYears = 0 - totalYears;
            }
            var targetValue = Convert.ToDouble(source.Value) * Math.Pow(Convert.ToDouble(1.00M + YearlyInflation), Convert.ToDouble(totalYears));

            return new Money(Convert.ToDecimal(targetValue), targetAt);
        }
    }
}