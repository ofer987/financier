using System;

namespace Financier.Common.Models
{
    public interface IInflation
    {
        Money GetValueAt(Money source, DateTime targetAt);
    }
}
