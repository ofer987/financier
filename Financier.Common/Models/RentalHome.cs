using System;
using System.Collections.Generic;

using Financier.Common.Extensions;

namespace Financier.Common.Models
{
    public class RentalHome : Product
    {
        public DateTime PurchasedAt { get; }
        public decimal MonthlyRate => Price;

        public RentalHome(string name, DateTime purchasedAt, decimal monthlyRate) : base(name, monthlyRate)
        {
            PurchasedAt = new DateTime(purchasedAt.Year, purchasedAt.Month, purchasedAt.Day);
        }

        public override decimal GetPurchasePrice(decimal price)
        {
            return 0.00M;
        }

        public override decimal GetSalePrice(decimal price, DateTime at)
        {
            return 0.00M;
        }

        public override IEnumerable<decimal> GetValueAt(DateTime at)
        {
            yield break;
        }

        public override IEnumerable<decimal> GetCostAt(DateTime at)
        {
            at = new DateTime(at.Year, at.Month, at.Day);

            if (at < PurchasedAt)
            {
                throw new ArgumentOutOfRangeException(nameof(at), at, $"should be equal or after PurchasedAt ({PurchasedAt})");
            }

            var months = at.SubtractWholeMonths(PurchasedAt) + 1;

            for (var i = 0; i < months; i += 1)
            {
                yield return MonthlyRate;
            }
        }
    }
}
