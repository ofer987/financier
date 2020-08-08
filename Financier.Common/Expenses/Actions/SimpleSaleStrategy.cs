namespace Financier.Common.Expenses.Actions
{
    public class SimpleSaleStrategy : ISaleStrategy
    {
        public decimal RequestedPrice { get; }

        public SimpleSaleStrategy(decimal requestedPrice)
        {
            RequestedPrice = requestedPrice;
        }

        public decimal GetReturnedPrice()
        {
            return RequestedPrice;
        }
    }
}
