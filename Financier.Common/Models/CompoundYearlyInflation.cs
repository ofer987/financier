using System;

namespace Financier.Common.Models
{
    public class CompoundYearlyInflation : IInflation
    {
        private const decimal DefaultRate = 0.02M;

        public decimal Rate { get; }

        public CompoundYearlyInflation(decimal rate = DefaultRate)
        {
            Rate = rate;
        }

        public decimal GetValueAt(decimal sourceValue, DateTime sourceAt, DateTime targetAt)
        {
            int totalYears;
            if (targetAt > sourceAt)
            {
                totalYears = Convert.ToInt32(
                    Math.Floor((targetAt - sourceAt).TotalDays / 365)
                );
            }
            else
            {
                totalYears = Convert.ToInt32(
                    Math.Floor((sourceAt - targetAt).TotalDays / 365)
                );
                totalYears = 0 - totalYears;
            }
            var targetValue = Convert.ToDouble(sourceValue) * Math.Pow(Convert.ToDouble(1.00M + Rate), Convert.ToDouble(totalYears));

            return new Money(Convert.ToDecimal(targetValue), targetAt);
        }
    }
}
