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

        public decimal ValueAt(int monthAfterInception)
        {
            var totalAssetValue = Assets
                .Select(asset => asset.ValueAt(monthAfterInception))
                .Aggregate(0.00M, (result, val) => result += val);

            var totalExpenseCost = Liabilities
                .Select(liability => liability.CostAt(monthAfterInception))
                .Aggregate(0.00M, (result, val) => result += val);

            return totalAssetValue - totalExpenseCost;
        }
    }
}
