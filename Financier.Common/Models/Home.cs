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

        public override decimal GetPurchasePrice(decimal price)
        {
            return 0.00M
                + new HomePurchaseStrategy(price, PurchasedAt).GetReturnedPrice() 
                + Financing.GetPurchasePrice(price);
        }

        public override decimal GetSalePrice(decimal price, DateTime at)
        {
            return 0.00M
                + new HomeSaleStrategy(price).GetReturnedPrice()
                - Financing.GetSalePrice(price, at);
        }

        public override IEnumerable<decimal> GetValueAt(DateTime at)
        {
            yield break;

        public override IEnumerable<decimal> GetCostAt(DateTime at)
        {
            return Financing.GetCostAt(at);
        }
    }
}
