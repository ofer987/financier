using System.Collections.Generic;

namespace Financier.Common.Expenses.Actions
{
    public class SimplePurchaseStrategy : IPurchaseStrategy
    {
        public decimal RequestedPrice { get; }

        public SimplePurchaseStrategy(decimal requested)
        {
            RequestedPrice = requested;
        }

        public IEnumerable<decimal> GetReturnedPrice()
        {
            yield return 0.00M - RequestedPrice;
        }
    }
}
