using System;

namespace Financier.Common.Models
{
    public class Home : Asset
    {
        // TODO: place in configuration file or should be configurable somehow
        public const decimal InflationRate = 2.00M;

        public decimal PurchasePrice { get; }

        public DateTime PurchasedAt { get; }

        public Home(decimal purchasePrice, DateTime purchasedAt)
        {
            PurchasePrice = purchasePrice;
            PurchasedAt = purchasedAt;
        }

        public decimal GetPriceAtYear(int year)
        {
            var purchasedAtYear = PurchasedAt.Year;
            if (year < purchasedAtYear)
            {
                throw new Exception($"The requested year ({year}) cannot be before the purchase year ({PurchasedAt})");
            }

            if (year == purchasedAtYear)
            {
                return PurchasePrice;
            }

            return Convert.ToDecimal(Math.Pow(Convert.ToDouble(InflationRate), year - purchasedAtYear) * Convert.ToDouble(PurchasePrice));
        }
    }
}
