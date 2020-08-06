using System.Collections.Generic;

using Financier.Common.Models;

namespace Financier.Common.Expenses.Actions
{
    public class MortgageSaleStrategy : ISaleStrategy
    {
        public decimal Requested { get; }

        public MortgageSaleStrategy(Money requested)
        {
            Requested = requested;
        }

        public IEnumerable<decimal> GetReturnedPrice()
        {
            // TODO use the same function HomeSaleStrategy#GetBankFees
            yield return Requested;
        }
    }
}
