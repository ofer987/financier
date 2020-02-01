using System;

namespace Financier.Common.Models
{
    public class Money
    {
        public decimal Value { get; }
        public DateTime At { get; }

        public Money(decimal val, DateTime at)
        {
            Value = val;
            At = at;
        }

        public Money GetValueAt(IInflation inflation, DateTime targetAt)
        {
            return inflation.GetValueAt(this, targetAt);
        }

        public static implicit operator decimal(Money money)
        {
            return money.Value;
        }
    }
}
