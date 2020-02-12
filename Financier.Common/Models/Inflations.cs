using System;

namespace Financier.Common.Models
{
    public static class Inflations
    {
        public static IInflation GetInflation(InflationTypes type)
        {
            switch (type)
            {
                case InflationTypes.NoopInflation:
                    return new NoopInflation();
                case InflationTypes.CompoundYearlyInflation:
                    return new CompoundYearlyInflation();
                default:
                    throw new ArgumentOutOfRangeException(nameof(type));
            }
        }
    }
}
