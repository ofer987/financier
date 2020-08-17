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
            var totalYears = 0;
            if (targetAt == sourceAt)
            {
                totalYears = 0;
            }
            else if (targetAt > sourceAt)
            {
                for (totalYears = 0; sourceAt.AddYears(totalYears) <= targetAt; totalYears += 1);

                totalYears -= 1;
            }
            else
            {
                for (totalYears = -1; sourceAt.AddYears(totalYears) >= targetAt; totalYears -= 1);

                totalYears += 1;
            }

            var targetValue = Convert.ToDouble(sourceValue) * Math.Pow(Convert.ToDouble(1.00M + Rate), Convert.ToDouble(totalYears));
            return new Money(Convert.ToDecimal(targetValue), targetAt);
        }
    }
}
