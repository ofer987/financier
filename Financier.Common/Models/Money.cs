using System;

using Financier.Common.Extensions;

namespace Financier.Common.Models
{
    public class Money
    {
        public static Money operator +(Money source, Money target)
        {
            if (!source.At.EqualsDate(target.At))
            {
                throw new InvalidOperationException($"Cannot decrement two Money amounts of different timestamps ({source.At}) and ({target.At})");
            }

            return new Money(source.Value + target.Value, source.At);
        }

        public static Money operator -(Money source, Money target)
        {
            if (!source.At.EqualsDate(target.At))
            {
                throw new InvalidOperationException($"Cannot decrement two Money amounts of different timestamps ({source.At}) and ({target.At})");
            }

            return new Money(source.Value - target.Value, source.At);
        }

        public static implicit operator decimal(Money money)
        {
            return money.Value;
        }

        public static implicit operator Money(decimal val) => new Money(val, DateTime.Now);

        public static Money Zero = new Money(0.00M, DateTime.MinValue);

        public DateTime At { get; }
        private decimal val;
        public decimal Value
        {
            get
            {
                return val;
            }

            set
            {
                val = decimal.Round(value, 2);
            }
        }

        public Money Reverse => new Money(0 - Value, At);

        public Money(decimal val, DateTime at)
        {
            Value = val;
            At = at.GetDate();
        }

        public Money GetValueAt(IInflation inflation, DateTime targetAt)
        {
            return inflation.GetValueAt(this, targetAt);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode() & At.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var other = obj as Money;
            if (other == null)
            {
                return false;
            }

            if (Value != other.Value || !At.EqualsDate(other.At))
            {
                return false;
            }

            return true;
        }

        public static bool operator ==(Money x, Money y)
        {
            if (object.ReferenceEquals(x, null))
            {
                return (object.ReferenceEquals(y, null));
            }

            return x.Equals(y);
        }

        public static bool operator !=(Money x, Money y)
        {
            return !(x == y);
        }

        public override string ToString()
        {
            return $"{Value} at {At}";
        }
    }
}
