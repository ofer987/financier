using System;

namespace Financier.Common.Models
{
    public static class Inflations
    {
        public static IInflation NoopInflation = new NoopInflation();
        public static IInflation ConsumerPriceIndex = new CompoundYearlyInflation(0.02M);
        public static IInflation CondoPriceIndex = new CompoundYearlyInflation(0.05M);
        public static IInflation TownHousePriceIndex = new CompoundYearlyInflation(0.08M);
        public static IInflation FreeHoldPriceIndex = new CompoundYearlyInflation(0.10M);

        public static IInflation GetInflation(InflationTypes type, params object[] args)
        {
            switch (type)
            {
                case InflationTypes.NoopInflation:
                    return NoopInflation;
                case InflationTypes.CompoundYearlyInflation:
                    if (args.Length >= 1)
                    {
                        return new CompoundYearlyInflation((decimal)args[0]);
                    }
                    else
                    {
                        return new CompoundYearlyInflation();
                    }
                default:
                    throw new ArgumentOutOfRangeException(nameof(type));
            }
        }
    }
}
