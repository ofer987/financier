using System;

namespace Financier.Common.Models.IncomeSources
{
    public class Salary
    {
        public decimal YearlyIncome { get; }

        public Salary(decimal yearlyIncome)
        {
            YearlyIncome = yearlyIncome;
        }
    }
}
