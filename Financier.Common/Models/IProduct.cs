using System;
using System.Collections.Generic;

namespace Financier.Common.Models
{
    public interface IProduct
    {
        Guid Id { get; }
        string Name { get; }

        Money Price { get; }

        IEnumerable<Money> GetValueAt(DateTime at);
    }
}
