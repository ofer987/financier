using System;
using System.Collections.Generic;

using Financier.Common.Models;

namespace Financier.Common.Expenses.Actions
{
    public class Purchase : Action
    {
        public override bool IsSold => Next.Type == Types.Sale;
        public override bool CanBuy => false;
        public override bool CanSell => true;
        public override decimal TransactionalPrice => decimal.Round(0.00M - Product.GetPurchasePrice(Price), 2);

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

        public override IEnumerable<decimal> GetValueAt(DateTime at)
        {
            if (IsSold && Next.At < at)
            {
                return Product.GetValueAt(Next.At);
            }

            return Product.GetValueAt(at);
        }

        public override IEnumerable<decimal> GetCostAt(DateTime at)
        {
            if (IsSold && Next.At < at)
            {
                return Product.GetCostAt(Next.At);
            }

            return Product.GetCostAt(at);
        }
    }
}
