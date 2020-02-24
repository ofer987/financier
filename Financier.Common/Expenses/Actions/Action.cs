using System;
using System.Collections.Generic;

using Financier.Common.Models;

namespace Financier.Common.Expenses.Actions
{
    public abstract class Action : IAction
    {
        public Types Type { get; }
        public IProduct Product { get; }
        public Money Price { get; }
        public virtual DateTime At { get; }

        public abstract bool IsSold { get; }
        public bool IsLastAction => Next.IsNull;
        public abstract bool CanBuy { get; }
        public abstract bool CanSell { get; }
        public virtual bool IsNull => false;

        protected IAction next = NullAction.Instance;
        public virtual IAction Next
        {
            get
            {
                return next;
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        protected Action(Types type, IProduct product, Money price, DateTime at)
        {
            Type = type;
            Product = product;
            Price = price;
            At = at;
        }

        protected Action(Types type)
        {
            Type = type;
        }

        public IEnumerable<IAction> GetActions()
        {
            for (IAction i = this; !i.IsNull; i = i.Next)
            {
                yield return i;
            }
        }
    }
}
