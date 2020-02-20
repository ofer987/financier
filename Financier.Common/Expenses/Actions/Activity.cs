using System;
using System.Linq;
using System.Collections.Generic;

using Financier.Common.Models;

namespace Financier.Common.Expenses.Actions
{
    public class Activity
    {
        private Dictionary<Guid, IAction> purchases = new Dictionary<Guid, IAction>();

        public IEnumerable<IAction> GetHistory(IProduct product)
        {
            if (!purchases.TryGetValue(product.Id, out var firstAction))
            {
                yield break;
            }

            foreach (var action in firstAction.GetActions())
            {
                yield return action;
            };
        }

        public void Buy(IProduct product, DateTime purchasedAt)
        {
            if (!purchases.TryGetValue(product.Id, out var firstAction))
            {
                purchases[product.Id] = new Purchase(product, purchasedAt); 

                return;
            }
            var lastAction = firstAction.GetActions().Last();

            if (purchasedAt < lastAction.At)
            {
                throw new ArgumentOutOfRangeException(nameof(purchasedAt), purchasedAt, $"The product {product} should not be purchased before ({lastAction.At})");
            }

            lastAction.Next = new Purchase(product, purchasedAt);
        }

        public void Sell(IProduct product, Money salePrice, DateTime soldAt)
        {
            if (!purchases.TryGetValue(product.Id, out var firstAction))
            {
                throw new InvalidOperationException($"Cannot sell the product {product} because it has not been purchased yet");
            }
            var lastAction = firstAction.GetActions().Last();

            if (soldAt < lastAction.At)
            {
                throw new ArgumentOutOfRangeException(nameof(soldAt), soldAt, $"The product {product} should not be sold before ({lastAction.At})");
            }

            lastAction.Next = new Sale(product, salePrice, soldAt);
        }
    }
}
