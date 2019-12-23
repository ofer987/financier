namespace Financier.Common.Models
{
    public class SimpleProduct : Product
    {
        public decimal Price { get; }
        public SimpleProduct(string name, decimal price) : base(name)
        {
            Price = price;
        }
    }
}
