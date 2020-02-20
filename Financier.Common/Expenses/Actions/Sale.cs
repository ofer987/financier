using System;

using Financier.Common.Models;

namespace Financier.Common.Expenses.Actions
{
    public class Sale : Action
    {
        public override bool IsSold => true;
        public override bool CanBuy => true;
        public override bool CanSell => false;

        public override IAction Next
        {
            get
            {
                return next;
            }

            set
            {
                if (value.Type != Types.Purchase)
                {
                    throw new InvalidOperationException($"The product {Product} should be purchased first");
                }

                next = value;
            }
        }

        public Sale(IProduct product, Money price, DateTime at) : base(Types.Sale, product, price, at)
        {
        }
    }
}
