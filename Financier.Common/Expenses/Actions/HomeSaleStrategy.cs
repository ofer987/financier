using System.Linq;
using System.Collections.Generic;

namespace Financier.Common.Expenses.Actions
{
    public class HomeSaleStrategy : ISaleStrategy
    {
        public decimal RequestedPrice { get; }

        public HomeSaleStrategy(decimal requestedPrice)
        {
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
            yield return 1000.00M;
        }
    }
}
