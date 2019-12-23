using System;
using System.Collections.Generic;

namespace Financier.Common.Models
{
    public interface IProduct
    {
        Guid Id { get; }
        string Name { get; }

        IList<IAsset> Assets { get; }
        IEnumerable<ILiability> Liabilities { get; }
    }
}
