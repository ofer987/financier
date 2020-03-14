namespace Financier.Common.Expenses.Actions
{
    public interface ISaleStrategy
    {
        decimal GetReturnedPrice(decimal requested);
    }
}
