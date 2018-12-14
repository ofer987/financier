using System;
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

        public decimal GetCost(int monthAfterInception)
        {
            return 0.00M;
        }
    }
}
