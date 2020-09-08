using System;
using System.Linq;
using System.Collections.Generic;

using Financier.Common.Models;

namespace Financier.Common.Expenses.Actions
{
    public class HomeSaleStrategy : ISaleStrategy
    {
        public static DateTime InflationStartsAt = new DateTime(2018, 1, 1);
        public DateTime RequestedAt { get; }
        public decimal RequestedPrice { get; }

        public HomeSaleStrategy(decimal requestedPrice, DateTime requestedAt)
        {
            RequestedAt = requestedAt;
            RequestedPrice = requestedPrice;
        }

        public decimal GetReturnedPrice()
        {
            return RequestedPrice - GetFees().Sum();
        }

        public IEnumerable<decimal> GetFees()
        {
            return Enumerable.Empty<decimal>()
                .Concat(GetRealtorFees())
                .Concat(GetLegalFees());
        }

        public IEnumerable<decimal> GetRealtorFees()
        {
            yield return 0.05M * RequestedPrice;
        }

        public IEnumerable<decimal> GetLegalFees()
        {
            yield return Inflations.ConsumerPriceIndex
                .GetValueAt(
                    1000.00M,
                    InflationStartsAt,
                    RequestedAt
                );
        }
    }
}
