using System;
using System.Collections.Generic;

namespace Financier.Common.Models
{
    public interface IProduct
    {
        Guid Id { get; }
        string Name { get; }

        List<IAsset> Assets { get; }
        List<ILiability> Liabilities { get; }
    }
}
