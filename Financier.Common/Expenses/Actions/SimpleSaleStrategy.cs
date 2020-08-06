using System.Collections.Generic;

namespace Financier.Common.Expenses.Actions
{
    public class SimpleSaleStrategy : ISaleStrategy
    {
        public decimal RequestedPrice { get; }

        public SimpleSaleStrategy(decimal requestedPrice)
        {
            RequestedPrice = requestedPrice;
        }

        public IEnumerable<decimal> GetReturnedPrice()
        {
            yield return RequestedPrice;
        }
    }
}
