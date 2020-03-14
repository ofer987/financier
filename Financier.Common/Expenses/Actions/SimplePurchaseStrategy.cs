using System.Collections.Generic;

using Financier.Common.Models;

namespace Financier.Common.Expenses.Actions
{
    public class SimplePurchaseStrategy : IPurchaseStrategy
    {
        public Money Requested { get; }

        public SimplePurchaseStrategy(Money requested)
        {
            Requested = requested;
        }

        public IEnumerable<Money> GetReturnedPrice()
        {
            yield return Requested;
        }
    }
}
