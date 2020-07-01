using System.Collections.Generic;

using Financier.Common.Models;

namespace Financier.Common.Expenses.Actions
{
    public class SimpleSaleStrategy : ISaleStrategy
    {
        public Money Requested { get; }

        public SimpleSaleStrategy(Money requested)
        {
            Requested = requested;
        }

        public IEnumerable<Money> GetReturnedPrice()
        {
            yield return Requested;
        }
    }
}
