using System;

namespace Financier.Common.Models
{
    public interface IProduct
    {
        DateTime PurchasedAt { get; }

        decimal PurchasePrice { get; }

        bool IsSold { get; }

        DateTime SoldAt { get; }

        decimal TotalBy(DateTime at);

        decimal ValueBy(DateTime at);

        decimal Sell(DateTime soldAt);
    }
}
