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

        public IEnumerable<IAction> GetHistories()
        {
            return purchases
                .Select(purchase => purchase.Key)
                .SelectMany(GetHistory);
        }

        public IEnumerable<IProduct> GetOwnedProducts(DateTime at)
        {
            foreach (var pair in purchases)
            {
                var product = pair.Key;

                var owned = false;
                foreach (var action in GetHistory(product).Where(action => action.At < at))
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
            if (purchases.TryGetValue(product, out var firstAction))
            {
                return firstAction.GetActions();
            }

            return Enumerable.Empty<IAction>();
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
            lastAction.Next = new Sale(product, salePrice, soldAt);
        }

        public decimal GetCash(IInflation inflation, DateTime startAt, DateTime endAt)
        {
            if (endAt < startAt)
            {
                throw new ArgumentOutOfRangeException(nameof(endAt), $"Should be at or later than {startAt}");
            }

            var result = 0.00M;
            result += GetCash(inflation, endAt);
            result -= GetCash(inflation, startAt);

            return decimal.Round(result, 2);
        }

        public decimal GetCash(IInflation inflation, DateTime at)
        {
            if (at < InitiatedAt)
            {
                throw new ArgumentOutOfRangeException(nameof(at), $"Should be at or later than {InitiatedAt}");
            }

            var result = 0.00M;
            result += InitialCash.GetValueAt(inflation, at);
            result -= InitialDebt.GetValueAt(inflation, at);
            result += CashFlow.DailyProfit * at.Subtract(InitiatedAt).Days;
            result += GetHistories()
                .Where(action => action.At >= InitiatedAt)
                .Where(action => action.At < at)
                .Select(action => action.CashFlow)
                .InflatedValue(inflation, at);


            return decimal.Round(result, 2);
        }

        public decimal GetAssets(IInflation inflation, DateTime at)
        {
            if (at < InitiatedAt)
            {
                throw new ArgumentOutOfRangeException(nameof(at), $"Should be at or later than {InitiatedAt}");
            }

            var result = 0.00M;
            result += InitialCash.GetValueAt(inflation, at);
            result += CashFlow.DailyProfit * at.Subtract(InitiatedAt).Days;
            result += GetHistories()
                .Where(action => action.At >= InitiatedAt)
                .Where(action => action.At < at)
                .Select(action => action.CashFlow)
                .InflatedValue(inflation, at);
            result += GetValueOfOwnedProducts(inflation, at);


            return decimal.Round(result, 2);
        }

        public decimal GetLiabilities(IInflation inflation, DateTime at)
        {
            if (at < InitiatedAt)
            {
                throw new ArgumentOutOfRangeException(nameof(at), $"Should be at or later than {InitiatedAt}");
            }

            var result = 0.00M;
            result += InitialDebt.GetValueAt(inflation, at).Value;
            result += GetCostOfOwnedProducts(inflation, at);

            return decimal.Round(result, 2);
        }

        public decimal GetNetWorth(IInflation inflation, DateTime at)
        {
            if (at < InitiatedAt)
            {
                throw new ArgumentOutOfRangeException(nameof(at), $"Should be at or later than {InitiatedAt}");
            }

            var result = 0.00M;
            result += GetAssets(inflation, at);
            result -= GetLiabilities(inflation, at);

            return result;
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
    }
}