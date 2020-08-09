using System;

namespace Financier.Common.Models
{
    public interface IProduct : IAsset, ILiability
    {
        Guid Id { get; }
        string Name { get; }

        decimal Price { get; }

        decimal GetPurchasePrice(decimal price);
        decimal GetSalePrice(decimal price, DateTime at);
    }
}
