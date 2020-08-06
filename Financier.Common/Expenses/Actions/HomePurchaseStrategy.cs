using System;
using System.Linq;
using System.Collections.Generic;

namespace Financier.Common.Expenses.Actions
{
    public class HomePurchaseStrategy : IPurchaseStrategy
    {
        public decimal PurchasePrice { get; }
        public DateTime PurchasedAt { get; }

        public HomePurchaseStrategy(decimal purchasePrice, DateTime purchasedAt)
        {
            PurchasePrice = purchasePrice;
            PurchasedAt = purchasedAt;
        }

        public IEnumerable<decimal> GetReturnedPrice()
        {
            yield return 0.00M - PurchasePrice;

            foreach (var fee in GetFees().Select(item => 0.00M - item))
            {
                yield return fee;
            }
        }

        public IEnumerable<decimal> GetFees()
        {
            return Enumerable.Empty<decimal>()
                .Concat(GetNotaryFees())
                .Concat(GetMunicipalTaxes())
                .Concat(GetMovingFees());
        }

        public IEnumerable<decimal> GetNotaryFees()
        {
            yield return 1000.00M;
        }

        public IEnumerable<decimal> GetMunicipalTaxes()
        {
            yield return 8500.00M;
        }

        public IEnumerable<decimal> GetMovingFees()
        {
            yield return 800.00M;
        }
    }
}
