using System;

using Financier.Common.Models;

namespace Financier.Common.Expenses.Actions
{
    public class Action : IAction
    {
        public Types Type { get; }
        public IProduct Product { get; }
        public decimal Price { get; }
        public DateTime At { get; }

        public Action(Types type, IProduct product, decimal price, DateTime at)
        {
            Type = type;
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
