using System;

using Financier.Common.Models;

namespace Financier.Common.Calculations
{
    public class MoneyByGivenDateTime
    {
        public IncomeStatement ByGivenDateTime(Person person, DateTime from, DateTime to)
        {
            return new IncomeStatement(0, person.IncomeSources, person.Products, from, to);
        }
    }
}
