using System;

using Financier.Common.Expenses.Actions;

namespace Financier.Common.Models
{
    public interface IProduct : IAsset, ILiability
    {
        Guid Id { get; }
        string Name { get; }

        Money Price { get; }

        IPurchaseStrategy GetPurchaseStrategy(Money price);
        ISaleStrategy GetSaleStrategy(Money price);
    }
}
