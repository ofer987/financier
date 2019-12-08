using System;

using Financier.Common.Models;

namespace Financier.Common.Expenses.Actions
{
    public class Sale : IAction
    {
        public Types Type => Types.Sale;
        public IProduct Product { get; }
        public decimal Price { get; }
        public DateTime At { get; }

        public Sale(IProduct product, decimal price, DateTime at)
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
