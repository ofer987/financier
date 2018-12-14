using System;
using System.Collections.Generic;

namespace Financier.Common.Models
{
    public class BalanceSheet
    {
        public List<Asset> Assets { get; }

        public List<Liability> Liability { get; }

        public BalanceSheet()
        {
        }
    }
}
