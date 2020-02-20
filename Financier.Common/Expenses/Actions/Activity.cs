using System;
using System.Linq;
using System.Collections.Generic;

using Financier.Common.Models;

namespace Financier.Common.Expenses.Actions
{
    public class Activity
    {
        private Dictionary<Product, IAction> purchases = new Dictionary<Product, IAction>();

        public IEnumerable<IEnumerable<IAction>> GetHistories()
        {
            return purchases
                .Select(purchase => purchase.Key)
                .Select(GetHistory);
        }

        public IEnumerable<Product> GetOwnedProducts(DateTime at)
        {
            foreach (var pair in purchases)
            {
                var product = pair.Key;

                var owned = false;
                // TODO: should the operator be exclusive or inclusive?
                foreach (var action in GetHistory(product).Where(action => action.At <= at))
                {
                    switch (action.Type)
                    {
                        case Types.Purchase:
                            owned = true;
                            break;
                        case Types.Sale:
                            owned = false;
                            break;
                        case Types.Null:
                            break;
                    }
                }

                if (owned)
                {
                    yield return product;
                }
            }
        }

        public IEnumerable<IAction> GetHistory(Product product)
        {
            if (!purchases.TryGetValue(product, out var firstAction))
            {
                yield break;
            }

            foreach (var action in firstAction.GetActions())
            {
                yield return action;
            };
        }

        public void Buy(Product product, DateTime purchasedAt)
        {
            if (!purchases.TryGetValue(product, out var firstAction))
            {
                purchases[product] = new Purchase(product, purchasedAt); 

                return;
            }
            var lastAction = firstAction.GetActions().Last();

            if (purchasedAt < lastAction.At)
            {
                throw new ArgumentOutOfRangeException(nameof(purchasedAt), purchasedAt, $"The product {product} should not be purchased before ({lastAction.At})");
            }

            lastAction.Next = new Purchase(product, purchasedAt);
        }

        public void Sell(Product product, Money salePrice, DateTime soldAt)
        {
            if (!purchases.TryGetValue(product, out var firstAction))
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
