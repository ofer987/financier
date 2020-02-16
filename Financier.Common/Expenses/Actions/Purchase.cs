using System;

using Financier.Common.Models;

namespace Financier.Common.Expenses.Actions
{
    public class Purchase : Action
    {
        public Purchase(IProduct product, DateTime at) : base(Types.Purchase, product, product.Price, at)
        {
        }
    }
}
