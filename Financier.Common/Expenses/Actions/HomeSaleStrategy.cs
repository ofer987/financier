using System;
using System.Linq;
using System.Collections.Generic;

namespace Financier.Common.Expenses.Actions
{
    public class HomeSaleStrategy : ISaleStrategy
    {
        public decimal RequestedPrice { get; }
        public DateTime RequestedAt { get; }

        public HomeSaleStrategy(decimal requestedPrice, DateTime requestedAt)
        {
            RequestedPrice = requestedPrice;
            RequestedAt = requestedAt;
        }

        public IEnumerable<decimal> GetReturnedPrice()
        {
            yield return RequestedPrice;

            foreach (var fee in GetFees().Select(item => 0.00M - item))
            {
                yield return fee;
            }
        }

        public IEnumerable<decimal> GetFees()
        {
            return Enumerable.Empty<decimal>()
                .Concat(GetRealtorFees());
        }

        public IEnumerable<decimal> GetRealtorFees()
        {
            yield return 0.05M * RequestedPrice;
        }
    }
}
