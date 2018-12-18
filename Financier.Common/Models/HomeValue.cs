namespace Financier.Common.Models
{
    public class HomeValue : Savings
    {
        public HomeValue(IProduct product, decimal purchasePrice) : base(product, purchasePrice)
        {
        }
    }
}
