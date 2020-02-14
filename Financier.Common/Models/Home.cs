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

        public Home(string name, DateTime purchasedAt, decimal downPayment) : base(name)
        {
            PurchasedAt = purchasedAt;
            DownPayment = new Money(downPayment, PurchasedAt);
        }

        public Home(string name, DateTime purchasedAt, decimal downPayment, IMortgage mortgage) : this(name, purchasedAt, downPayment)
        {
            Financing = mortgage;
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
            yield return Financing.GetBalance(at);
        }
    }
}
