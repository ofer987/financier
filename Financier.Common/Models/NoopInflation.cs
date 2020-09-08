using System;

namespace Financier.Common.Models
{
    public class NoopInflation : IInflation
    {
        public decimal GetValueAt(decimal sourceValue, DateTime _sourceAt, DateTime _targetAt)
        {
            return sourceValue;
        }
    }
}
