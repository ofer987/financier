using System;
using System.Linq;
using System.Collections.Generic;

using Financier.Common.Liabilities;

namespace Financier.Common.Models
{
    public class Home : Product
    {
        public DateTime PurchasedAt { get; }
        public decimal Valuation { get; }
        public Money DownPayment { get; }
        public IMortgage Financing { get; }

        public Home(string name, DateTime purchasedAt, Money purchasePrice, Money downPayment, IMortgage mortgage) : this(name, purchasedAt, purchasePrice, downPayment)
        {
            Financing = mortgage;
        }

        public Home(string name, DateTime purchasedAt, Money purchasePrice, Money downPayment) : base(name, purchasePrice)
        {
            PurchasedAt = purchasedAt;
            DownPayment = downPayment;
        }

        public override IEnumerable<Money> GetValueAt(DateTime at)
        {
            yield return DownPayment;

            foreach (var principal in Financing.GetMonthlyPayments(at).Select(payment => payment.Principal))
            {
                yield return principal;
            }
        }

        public override IEnumerable<Money> GetCostAt(DateTime at)
        {
            return Enumerable.Empty<Money>();
        }
    }
}
