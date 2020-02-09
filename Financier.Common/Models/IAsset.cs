using System;

namespace Financier.Common.Models
{
    public interface IAsset
    {
        decimal PurchasePrice { get; }

        decimal ValueAt(int monthAfterInception);

        decimal ValueAt(DateTime at);

        decimal ValueBy(int monthAfterInception);

        decimal ValueBy(DateTime at);
    }
}
