using System;

using Financier.Common.Models;

namespace Financier.Common.Expenses.Actions
{
    public class Sale : Action
    {
        public Sale(IProduct product, Money price, DateTime at) : base(Types.Sale, product, price, at)
        {
        }
    }
}
