using System;

namespace Financier.Common.Models
{
    public interface IInflation
    {
        decimal GetValueAt(decimal sourceValue, DateTime sourceAt, DateTime targetAt);
    }
}
