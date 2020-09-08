using System;
using System.Linq;
using System.Collections.Generic;

using Financier.Common.Models;

namespace Financier.Common.Expenses.Actions
{
    public class HomePurchaseStrategy : IPurchaseStrategy
    {
        public static DateTime InflationStartsAt = new DateTime(2018, 1, 1);

        public decimal PurchasePrice { get; }
        public DateTime PurchasedAt { get; }

        public HomePurchaseStrategy(decimal purchasePrice, DateTime purchasedAt)
        {
            PurchasePrice = purchasePrice;
            PurchasedAt = purchasedAt;
        }

        public decimal GetReturnedPrice()
        {
            return 0.00M
                + PurchasePrice
                + GetFees().Sum();
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
            yield return Inflations.ConsumerPriceIndex.GetValueAt(
                1000.00M,
                InflationStartsAt,
                PurchasedAt
            );
        }

        public IEnumerable<decimal> GetMunicipalTaxes()
        {
            yield return Inflations.ConsumerPriceIndex.GetValueAt(
                8500.00M,
                InflationStartsAt,
                PurchasedAt
            );
        }

        public IEnumerable<decimal> GetMovingFees()
        {
            yield return Inflations.ConsumerPriceIndex.GetValueAt(
                800.00M,
                InflationStartsAt,
                PurchasedAt
            );
        }
    }
}
