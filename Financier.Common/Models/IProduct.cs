using System;

namespace Financier.Common.Models
{
    public interface IProduct : IAsset, ILiability
    {
        Guid Id { get; }
        string Name { get; }

        Money Price { get; }
    }
}
