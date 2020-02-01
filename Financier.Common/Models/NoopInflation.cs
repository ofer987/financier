using System;

public class NoopInflation : IInflation
{
    public Money GetValueAt(Money source, DateTime targetAt)
    {
        return source;
    }
}
