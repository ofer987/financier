using System;

using Financier.Common.Calculations;
using Financier.Common.Models.Expenses;

namespace Financier.Common.Models
{
    public class RentedHome : ILiability
    {
        // TODO: place in configuration file or should be configurable somehow
        public const decimal YearlyInflationRate = 2.00M;

        // TODO place in a configuration file or pass in as constructor parameter
        public const decimal YearlyValuationRate = 5.00M;

        public double MonthlyInflationRate => Math.Pow(Convert.ToDouble(YearlyInflationRate) / 100, 1.0/12) - 1;

        public DateTime RentedAt { get; }

        public MonthlyExpenses Expenses { get; }

        public RentedHome(DateTime rentedAt, MonthlyExpenses expenses)
        {
            RentedAt = rentedAt;
            Expenses = expenses;
        }

        public decimal GetTotalYearlyExpenses(int monthAfterInception)
        {
            if (monthAfterInception < 0)
            {
                throw new Exception($"{nameof(monthAfterInception)} cannot be negative number");
            }

            var result = 0.00M;

            for (var i = monthAfterInception; i < monthAfterInception + 12; i += 1)
            {
                result += Expenses.MonthlyTotal;
            }
            return result;
        }

        public decimal CostAt(int monthAfterInception)
        {
            if (monthAfterInception < 0)
            {
                throw new Exception($"{nameof(monthAfterInception)} cannot be negative number");
            }

            return Expenses.MonthlyTotal;
        }

        public decimal CostBy(int monthAfterInception)
        {
            if (monthAfterInception < 0)
            {
                throw new Exception($"{nameof(monthAfterInception)} cannot be negative number");
            }

            return monthAfterInception * Expenses.MonthlyTotal;
        }
    }
}
