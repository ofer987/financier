using System;
using System.Linq;
using System.Collections.Generic;

namespace Financier.Common.Models.Expenses
{
    public class MonthlyExpenses : Dictionary<string, decimal>
    {
        public decimal MonthlyTotal
        {
            get
            {
                var total = this
                    .Select(pair => Convert.ToDouble(pair.Value))
                    .Aggregate(0.00, (result, val) => result += val);

                return Convert.ToDecimal(total);
            }
        }

        public decimal YearlyTotal => 12 * MonthlyTotal;
    }
}
