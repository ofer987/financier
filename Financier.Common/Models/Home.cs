using System;

using Financier.Common.Calculations;

namespace Financier.Common.Models
{
    public class Home : Product
    {
        public DateTime PurchasedAt { get; }
        public decimal Valuation { get; }
        public decimal DownPayment { get; }
        public FixedRateMortgage Financing { get; } = null;

        public Home(string name, DateTime purchasedAt, decimal downPayment, decimal valuation) : base(name)
        {
            PurchasedAt = purchasedAt;
            DownPayment = downPayment;
            Valuation = valuation;
        }

        public Home(string name, DateTime purchasedAt, decimal downPayment, decimal valuation, FixedRateMortgage mortgage) : this(name, purchasedAt, downPayment, valuation)
        {
            Financing = mortgage;
            Liabilities.Add(Financing);
        }

        public decimal GetValueBy(int months)
        {
            return 0.00M
                + DownPayment 
                + Financing.GetPrincipalPaymentsBy(months);
        }

        public decimal GetRemainingMortgageAmount(int months)
        {
            return Financing.GetBalance(months);
        }
    }
}
