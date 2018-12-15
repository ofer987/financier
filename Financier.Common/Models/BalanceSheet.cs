using System;
using System.Linq;
using System.Collections.Generic;

namespace Financier.Common.Models
{
    public class BalanceSheet
    {
        public List<IAsset> Assets { get; }

        public List<ILiability> Liabilities { get; }

        public BalanceSheet()
        {
        }

        public decimal ValueBy(int monthAfterInception)
        {
            if (monthAfterInception < 0)
            {
                throw new Exception($"{nameof(monthAfterInception)} cannot be negative number");
            }

            var totalAssetValue = Assets
                .Select(asset => asset.ValueBy(monthAfterInception))
                .Aggregate(0.00M, (result, val) => result += val);

            var totalExpenseCost = Liabilities
                .Select(liability => liability.CostBy(monthAfterInception))
                .Aggregate(0.00M, (result, val) => result += val);

            return totalAssetValue - totalExpenseCost;
        }
    }
}
