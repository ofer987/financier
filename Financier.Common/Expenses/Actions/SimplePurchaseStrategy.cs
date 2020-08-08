namespace Financier.Common.Expenses.Actions
{
    public class SimplePurchaseStrategy : IPurchaseStrategy
    {
        public decimal RequestedPrice { get; }

        public SimplePurchaseStrategy(decimal requested)
        {
            RequestedPrice = requested;
        }

        public decimal GetReturnedPrice()
        {
            return 0.00M - RequestedPrice;
        }
    }
}
