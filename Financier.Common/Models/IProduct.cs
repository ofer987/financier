using System;

using Financier.Common.Expenses.Actions;

namespace Financier.Common.Models
{
    public interface IProduct : IAsset, ILiability
    {
        Guid Id { get; }
        string Name { get; }

        decimal Price { get; }

        // TODO: replace with return type decimal
        IPurchaseStrategy GetPurchaseStrategy(decimal price);

        decimal GetSalePrice(decimal price, DateTime at);
    }
}
