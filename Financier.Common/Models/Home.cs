using System;
using System.Collections.Generic;

using Financier.Common.Expenses.Actions;
using Financier.Common.Liabilities;

namespace Financier.Common.Models
{
    public class Home : Product
    {
        public DateTime PurchasedAt { get; }
        public decimal Valuation { get; }
        public decimal DownPayment { get; }
        public IMortgage Financing { get; }

        public Home(string name, DateTime purchasedAt, decimal purchasePrice, decimal downPayment, IMortgage mortgage) : this(name, purchasedAt, purchasePrice, downPayment)
        {
            Financing = mortgage;
        }

        public Home(string name, DateTime purchasedAt, decimal purchasePrice, decimal downPayment) : base(name, purchasePrice)
        {
            PurchasedAt = purchasedAt;
            DownPayment = downPayment;
        }

        public override IPurchaseStrategy GetPurchaseStrategy(decimal price)
        {
            return new HomePurchaseStrategy(price, PurchasedAt);
        }

        public override ISaleStrategy GetSaleStrategy(decimal price, DateTime at)
        {
            // TODO: need to pass the sale price. This is the purchase price
            return new HomeSaleStrategy(price, at);
        }

        public override IEnumerable<decimal> GetValueAt(DateTime at)
        {
            if (PurchasedAt < at)
            {
                yield return DownPayment;
            }

            foreach (var payment in Financing.GetMonthlyPayments(at))
            {
                yield return payment.Principal;
            }
        }

        public override IEnumerable<decimal> GetCostAt(DateTime at)
        {
            yield break;
        }
    }
}
