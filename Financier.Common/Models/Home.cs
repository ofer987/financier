using System;
using System.Collections.Generic;

using Financier.Common.Expenses.Actions;
using Financier.Common.Liabilities;
using Financier.Common.Extensions;

namespace Financier.Common.Models
{
    public class Home : Product
    {
        public DateTime PurchasedAt { get; }
        public decimal Valuation { get; }
        public decimal DownPayment { get; }
        public IMortgage Financing { get; }
        public decimal MonthlyMaintenanceFees { get; }

        public Home(string name, DateTime purchasedAt, decimal purchasePrice, decimal downPayment, IMortgage mortgage, decimal monthlyMaintenanceFees = 0.00M) : base(name, purchasePrice)
        {
            Financing = mortgage;
            PurchasedAt = new DateTime(purchasedAt.Year, purchasedAt.Month, purchasedAt.Day);
            DownPayment = downPayment;
            MonthlyMaintenanceFees = monthlyMaintenanceFees;
        }

        public override decimal GetPurchasePrice(decimal price)
        {
            return 0.00M
                + new HomePurchaseStrategy(price, PurchasedAt).GetReturnedPrice()
                + Financing.GetPurchasePrice(price);
        }

        public override decimal GetSalePrice(decimal price, DateTime at)
        {
            return 0.00M
                + new HomeSaleStrategy(price, at).GetReturnedPrice()
                - Financing.GetSalePrice(price, at);
        }

        public override IEnumerable<decimal> GetValueAt(DateTime at)
        {
            yield break;
        }

        public override IEnumerable<decimal> GetCostAt(DateTime at)
        {
            return Financing.GetCostAt(at);
        }

        public IEnumerable<decimal> GetMaintenancePayments(DateTime at)
        {
            at = new DateTime(at.Year, at.Month, at.Day);

            if (at <= PurchasedAt)
            {
                yield break;
            }

            for (var i = 0; i < at.SubtractMonths(PurchasedAt); i += 1)
            {
                yield return Inflations.ConsumerPriceIndex
                    .GetValueAt(
                        MonthlyMaintenanceFees,
                        PurchasedAt,
                        PurchasedAt.AddMonths(i)
                    );
            }
        }
    }
}
