using System;

namespace Financier.Common.Models
{
    public class Money
    {
        public static Money operator +(Money source, Money target)
        {
            if (source.At != target.At)
            {
                throw new InvalidOperationException("Cannot decrement two Money amounts of different timestamps");
            }

            return new Money(source.Value + target.Value, source.At);
        }

        public static Money operator -(Money source, Money target)
        {
            if (source.At != target.At)
            {
                throw new InvalidOperationException("Cannot decrement two Money amounts of different timestamps");
            }

            return new Money(source.Value - target.Value, source.At);
        }

        public static implicit operator decimal(Money money)
        {
            return money.Value;
        }

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
    }
}
