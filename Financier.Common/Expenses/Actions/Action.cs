using System;

using Financier.Common.Models;

namespace Financier.Common.Expenses.Actions
{
    public abstract class Action : IAction
    {
        public Types Type { get; }
        public IProduct Product { get; }
        public Money Price { get; }
        public DateTime At { get; }

        public Action(Types type, IProduct product, Money price, DateTime at)
        {
            Type = type;
            Product = product;
            Price = price;
            At = at;
        }
    }
}
