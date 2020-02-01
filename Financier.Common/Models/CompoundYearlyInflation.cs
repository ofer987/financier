using System;

public class CompoundYearlyInflation : IInflation
{
    public const decimal YearlyInflation = 0.02M;

    public Money GetValueAt(Money source, DateTime targetAt)
    {
        var totalYears = (targetAt - source.At).TotalDays / 365;
        var targetValue = Math.Pow(Convert.ToDouble(source.Value), Convert.ToDouble(totalYears));

        return new Money(Convert.ToDecimal(targetValue), targetAt);
    }
}
