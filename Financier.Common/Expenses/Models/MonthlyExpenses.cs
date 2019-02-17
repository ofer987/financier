using System;
using System.Linq;
using System.Collections.Generic;

namespace Financier.Common.Models.Expenses
{
    public class MonthlyExpenses : Liability
    {
        // TODO: place in configuration file or should be configurable somehow
        public const decimal YearlyInflationRate = 2.00M;

        public double EffectiveInterestRateMonthly => Math.Pow(Math.Pow((Convert.ToDouble(YearlyInflationRate) / (1 * 100) + 1), 1), 1.0/12) - 1;

        public Dictionary<string, decimal> Values { get; } = new Dictionary<string, decimal>();

        public decimal MonthlyTotal
        {
            get
            {
                var total = Values
                    .Select(pair => Convert.ToDouble(pair.Value))
                    .Aggregate(0.00, (result, val) => result += val);

                return Convert.ToDecimal(total);
            }
        }

        public decimal YearlyTotal => 12 * MonthlyTotal;

        public MonthlyExpenses(IProduct product, IDictionary<string, decimal> initialExpenses)  : base(product)
        {
            // NOTE Or should I simply assign initialExpenses to Expenses?
            foreach (var pair in initialExpenses)
            {
                Values.Add(pair.Key, pair.Value);
            }
        }

        public override decimal CostAt(int monthAfterInception)
        {
            return MonthlyTotal * Convert.ToDecimal(EffectiveInterestRateMonthly) * monthAfterInception;
        }

        public override decimal CostBy(int monthAfterInception)
        {
            var result = 0.00M;
            for (var i = 0; i < monthAfterInception; i += 1)
            {
                result += MonthlyTotal * monthAfterInception;
            }

            return result;
        }
    }
}
