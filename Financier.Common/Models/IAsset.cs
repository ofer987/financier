using System;
using System.Collections.Generic;

namespace Financier.Common.Models
{
    public interface IAsset
    {
        IEnumerable<decimal> GetValueAt(DateTime at);
    }
}
