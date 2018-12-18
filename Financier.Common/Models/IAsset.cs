using System;

namespace Financier.Common.Models
{
    public interface IAsset
    {
        IProduct Product { get; }

        decimal PurchasePrice { get; }

        bool IsSold { get; }

        decimal SellPrice { get; }

        decimal ValueAt(int monthAfterInception);

        decimal ValueAt(DateTime at);

        decimal ValueBy(int monthAfterInception);

        decimal ValueBy(DateTime at);
    }
}
