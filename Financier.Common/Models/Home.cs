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

        public override IEnumerable<ILiability> Liabilities
        {
            get
            {
                yield return Financing;
            }
        }

        public Home(string name, DateTime purchasedAt, decimal downPayment) : base(name)
        {
            PurchasedAt = purchasedAt;
            DownPayment = new Money(downPayment, PurchasedAt);
        }

        public Home(string name, DateTime purchasedAt, decimal downPayment, IMortgage mortgage) : this(name, purchasedAt, downPayment)
        {
            Financing = mortgage;
        }

        public decimal GetValueBy(int months)
        {
            return 0.00M
                + DownPayment 
                + Financing.GetMonthlyPayments(PurchasedAt.AddMonths(months))
                    .Select(payment => payment.Principal.Value)
                    .Sum();
        }

        public decimal GetRemainingMortgageAmount(int months)
        {
            return Financing.GetMonthlyPayments(PurchasedAt.AddMonths(months))
                .Select(payment => payment.Balance)
                .Last();
        }
    }
}
