using System;

namespace Financier.Common.Models
{
    public class NoopInflation : IInflation
    {
        public Money GetValueAt(Money source, DateTime targetAt)
        {
            return source;
        }
    }
}
