using System;

namespace Financier.Common.Models
{
    public interface IProduct
    {
        Guid Id { get; }
        string Name { get; }

        Money Price { get; }
    }
}
