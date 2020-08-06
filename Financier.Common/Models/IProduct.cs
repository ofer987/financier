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

        // TODO: replace with return type decimal
        ISaleStrategy GetSaleStrategy(decimal price, DateTime at);
    }
}
