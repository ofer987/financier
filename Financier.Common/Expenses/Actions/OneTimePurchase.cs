using System;

using Financier.Common.Models;

namespace Financier.Common.Expenses.Actions
{
    public class OneTimeAction : Action, IOnce
    {
        public OneTimeAction(Types type, SimpleProduct product, DateTime at) : base(type, product, product.Price, at)
        {
        }

        public OneTimeAction(Types type, SimpleProduct product) : base(type, product, product.Price, DateTime.Now)
        {
        }
    }
}
