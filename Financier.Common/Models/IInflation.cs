using System;

public interface IInflation
{
    Money GetValueAt(Money source, DateTime targetAt);
}
