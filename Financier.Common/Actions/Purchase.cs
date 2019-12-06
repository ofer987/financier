using System;

using Financier.Common.Models;

namespace Financier.Common.Actions
{
    public class Purchase : IAction
    {
        public Types Type => Types.Purchase;
        public IProduct Product { get; }
        public decimal Price { get; }
        public DateTime At { get; }

        public Purchase(IProduct product, decimal price, DateTime at)
        {
            Product = product;
            Price = price;
            At = at;
        }

        public decimal PriceAt(DateTime at)
        {
            return Price;
        }
    }
}
