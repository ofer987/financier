using System;

namespace Financier.Common.Models
{
    public interface IProduct
    {
        DateTime PurchasedAt { get; }

        decimal TotalBy(DateTime at);
    }
}
