using System;
using System.Collections.Generic;

namespace Financier.Common.Models
{
    public interface ILiability
    {
        IEnumerable<decimal> GetCostAt(DateTime at);
    }
}
