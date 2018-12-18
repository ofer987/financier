using System;
using System.Linq;
using System.Collections.Generic;

namespace Financier.Common.Models.Expenses
{
    public class MonthlyExpenses : Liability
    {
        public Dictionary<string, decimal> Expenses { get; } = new Dictionary<string, decimal>();

        public decimal MonthlyTotal
        {
            get
            {
                var total = Expenses
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
                Expenses.Add(pair.Key, pair.Value);
            }
        }

        public override decimal CostAt(int monthAfterInception)
        {
            return MonthlyTotal;
        }

        public override decimal CostBy(int monthAfterInception)
        {
            return MonthlyTotal * monthAfterInception;
        }
    }
}
