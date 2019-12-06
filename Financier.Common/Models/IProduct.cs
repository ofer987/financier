using System;
using System.Collections.Generic;

namespace Financier.Common.Models
{
    public interface IProduct
    {
        Guid Id { get; }

        string Name { get; }

        DateTime PurchasedAt { get; }

        decimal PurchasePrice { get; }

        decimal InvestmentPrice { get; }

        List<IAsset> Assets { get; }

        List<ILiability> Liabilities { get; }

        void Sell(decimal price, DateTime at);

        bool IsSold { get; }

        DateTime SoldAt { get; }

        decimal ValueBy(DateTime at);

        decimal CostBy(DateTime at);
    }
}
