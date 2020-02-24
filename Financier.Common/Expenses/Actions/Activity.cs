using System;
using System.Linq;
using System.Collections.Generic;

using Financier.Common.Extensions;
using Financier.Common.Models;

namespace Financier.Common.Expenses.Actions
{
    public class Activity
    {
        public DateTime InitiatedAt { get; set; }
        public Money InitialCash { get; set; } = Money.Zero;
        public Money InitialDebt { get; set; } = Money.Zero;
        public ICashFlow CashFlow { get; set; }

        private Dictionary<IProduct, IAction> purchases = new Dictionary<IProduct, IAction>();

        public Activity(DateTime initiatedAt)
        {
            CashFlow = new DummyCashFlow(0.00M);
            InitiatedAt = InitiatedAt;
        }

        public Activity(ICashFlow cashFlow, DateTime initiatedAt)
        {
            CashFlow = cashFlow;
            InitiatedAt = initiatedAt;
        }

        public Activity(Money cash, Money debt, ICashFlow cashFlow, DateTime initiatedAt)
        {
            InitiatedAt = initiatedAt;
            InitialCash = cash;
            InitialDebt = debt;

            CashFlow = cashFlow;
        }

        public IEnumerable<IEnumerable<IAction>> GetHistories()
        {
            return purchases
                .Select(purchase => purchase.Key)
                .Select(GetHistory);
        }

        public IEnumerable<IProduct> GetOwnedProducts(DateTime at)
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

        public IEnumerable<IAction> GetHistory(IProduct product)
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

        public void Buy(IProduct product, DateTime purchasedAt)
        {
            if (purchasedAt < InitiatedAt)
            {
                throw new ArgumentOutOfRangeException(nameof(purchasedAt), purchasedAt, $"The product cannot be purchased before {InitiatedAt}");
            }

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

        public void Sell(IProduct product, Money salePrice, DateTime soldAt)
        {
            if (soldAt < InitiatedAt)
            {
                throw new ArgumentOutOfRangeException(nameof(soldAt), soldAt, $"The product cannot be sold before {InitiatedAt}");
            }

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

        public decimal GetValueOfOwnedProducts(IInflation inflation, DateTime at)
        {
            return GetOwnedProducts(at)
                .SelectMany(product => product.GetValueAt(at))
                .InflatedValue(inflation, at);
        }

        public decimal GetCostOfOwnedProducts(IInflation inflation, DateTime at)
        {
            return GetOwnedProducts(at)
                .SelectMany(product => product.GetCostAt(at))
                .InflatedValue(inflation, at);
        }

        public decimal GetCashAt(DateTime at)
        {
            // TODO: or maybe it should construct a balance sheet to know cash reserves?
            if (at < InitiatedAt)
            {
                throw new ArgumentOutOfRangeException(nameof(at), $"Should be at or later than {InitiatedAt}");
            }

            return 0.00M
                + InitialCash.Value
                - InitialDebt
                + CashFlow.DailyProfit * at.Subtract(InitiatedAt).Days;
        }
    }
}
