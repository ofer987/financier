namespace Financier.Common.Models
{
    public class HomeValue : Savings
    {
        public override decimal InvestmentPrice { get; }

        public HomeValue(IProduct product, decimal purchasePrice, decimal investmentPrice) : base(product, purchasePrice)
        {
            InvestmentPrice = investmentPrice;
        }
    }
}
