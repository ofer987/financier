using System;

using Financier.Common.Extensions;

namespace Financier.Common.Models.Income
{
    public class Salary : Base
    {
        public decimal Yearly { get; }

        public decimal Monthly => Yearly / 12;

        public Salary(decimal yearly)
        {
            Yearly = yearly;
        }

        // TODO compute daily salary accounting for business days per year
        public override decimal Value(DateTime from, DateTime to)
        {
            if (to < from)
            {
                throw new Exception($"Salary cannot be computed in reverse order from ({from}) to ({to})");
            }

            return to.SubtractWholeMonths(from) * Monthly;
        }
    }
}
