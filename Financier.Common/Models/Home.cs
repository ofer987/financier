using System;

namespace Financier.Common.Models
{
    public class Home : Product
    {
        public DateTime PurchasedAt { get; }
        public decimal Valuation { get; }
        public decimal DownPayment { get; }

        public Home(string name, DateTime purchasedAt, decimal downPayment, decimal valuation) : base(name)
        {
            PurchasedAt = purchasedAt;
            DownPayment = downPayment;
            Valuation = valuation;
        }
    }
}
