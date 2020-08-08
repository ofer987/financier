using System;

using Financier.Common.Models;

namespace Financier.Common.Expenses.Actions
{
    public class Purchase : Action
    {
        public override bool IsSold => Next.Type == Types.Sale;
        public override bool CanBuy => false;
        public override bool CanSell => true;
        public override decimal TransactionalPrice => Product.GetPurchaseStrategy(Price).GetReturnedPrice();

        public override IAction Next
        {
            get
            {
                return next;
            }

            set
            {
                if (value.Type != Types.Sale)
                {
                    throw new InvalidOperationException($"The product {Product} should be sold first");
                }

                if (value.At < At)
                {
                    throw new ArgumentOutOfRangeException(nameof(value.At), value.At, $"The product {value.Product} should not be purchased before ({At})");
                }

                next = value;
            }
        }

        public Purchase(IProduct product, DateTime at) : base(Types.Purchase, product, product.Price, at)
        {
        }
    }
}
