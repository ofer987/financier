using System;
using System.Linq;
using System.Collections.Generic;

using Financier.Common.Models;

namespace Financier.Common.Expenses.Actions
{
    public class HomeSaleStrategy : ISaleStrategy
    {
        public Money Requested { get; }
        public DateTime At => Requested.At;

        public HomeSaleStrategy(Money requested)
        {
            Requested = requested;
        }

        public IEnumerable<Money> GetReturnedPrice()
        {
            yield return Requested;

            foreach (var fee in GetFees().Select(item => item.Reverse))
            {
                yield return fee;
            }
        }

        public IEnumerable<Money> GetFees()
        {
            return Enumerable.Empty<Money>()
                .Concat(GetRealtorFees());
        }

        public IEnumerable<Money> GetRealtorFees()
        {
            yield return new Money(0.05M * Requested.Value, At);
        }
    }
}
