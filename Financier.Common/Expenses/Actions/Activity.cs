using System;
using System.Linq;
using System.Collections.Generic;

using Financier.Common.Models;

namespace Financier.Common.Expenses.Actions
{
    public class Activity : ICashFlow
    {
        public DateTime InitiatedAt { get; }
        public decimal InitialCash { get; set; }
        public decimal InitialDebt { get; set; }
        public ICashFlow CashFlow { get; set; }

        decimal? dailyProfit = null;
        public decimal DailyProfit
        {
            get
            {
                if (dailyProfit.HasValue)
                {
                    return dailyProfit.Value;
                }

                var endAt = GetHistories().Last().At;
                dailyProfit = GetCashAt(Inflations.NoopInflation, endAt);

                return dailyProfit.Value;
            }
        }

        private Dictionary<IProduct, IAction> purchases = new Dictionary<IProduct, IAction>();

        public Activity(DateTime initiatedAt)
        {
            CashFlow = new DummyCashFlow(0.00M);
            InitiatedAt = initiatedAt;
        }

        public Activity(ICashFlow cashFlow, DateTime initiatedAt)
        {
            CashFlow = cashFlow;
            InitiatedAt = initiatedAt;
        }

        public Activity(decimal cash, decimal debt, ICashFlow cashFlow, DateTime initiatedAt)
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

        public IEnumerable<IProduct> GetOwnedProducts()
        {
            return GetOwnedProducts(DateTime.MaxValue);
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

        public void Sell(IProduct product, decimal salePrice, DateTime soldAt)
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

        // Refactor me
        public decimal GetCash(DateTime startAt, DateTime endAt)
        {
            if (endAt <= startAt)
            {
                throw new ArgumentOutOfRangeException(nameof(endAt), endAt, $"Should be later than {startAt}");
            }

            var result = 0.00M;
            result += CashFlow.DailyProfit * endAt.Subtract(startAt).Days;
            result += GetHistories()
                .Where(action => action.At >= startAt)
                .Where(action => action.At < endAt)
                .Select(action => action.TransactionalPrice)
                .Sum();

            return decimal.Round(result, 2);
        }

        public decimal GetCash(IInflation inflation, DateTime at)
        {
            if (at <= InitiatedAt)
            {
                throw new ArgumentOutOfRangeException(nameof(at), at, $"Should be later than {InitiatedAt}");
            }

            var result = 0.00M;
            result += InitialCash;
            result -= InitialDebt;
            result += CashFlow.DailyProfit * at.Subtract(InitiatedAt).Days;
            result += GetHistories()
                .Where(action => action.At >= InitiatedAt)
                .Where(action => action.At < at)
                .Select(action => action.TransactionalPrice)
                .Sum();

            return decimal.Round(result, 2);
        }

        public decimal GetValueAt(DateTime at)
        {
            if (at <= InitiatedAt)
            {
                throw new ArgumentOutOfRangeException(nameof(at), $"Should be later than {InitiatedAt}");
            }

            var result = 0.00M;
            result += GetHistories()
                .SelectMany(item => item.GetValueAt(at))
                .Sum();

            return result;
        }

        public decimal GetCostAt(DateTime at)
        {
            if (at <= InitiatedAt)
            {
                throw new ArgumentOutOfRangeException(nameof(at), $"Should be later than {InitiatedAt}");
            }

            var result = 0.00M;
            result += GetHistories()
                .SelectMany(item => item.GetCostAt(at))
                .Sum();

            return result;
        }

        public decimal GetCashAt(IInflation inflation, DateTime at)
        {
            if (at <= InitiatedAt)
            {
                throw new ArgumentOutOfRangeException(nameof(at), $"Should be later than {InitiatedAt}");
            }

            var result = 0.00M;
            result += InitialCash;
            result -= InitialDebt;
            // Adjust for inflation
            result += CashFlow.DailyProfit * at.Subtract(InitiatedAt).Days;
            result += GetHistories()
                .Where(action => action.At >= InitiatedAt)
                .Where(action => action.At < at)
                .Select(action => action.TransactionalPrice)
                .Sum();

            return decimal.Round(result, 2);
        }

        public decimal GetNetWorthAt(IInflation inflation, DateTime at)
        {
            if (at <= InitiatedAt)
            {
                throw new ArgumentOutOfRangeException(nameof(at), $"Should be later than {InitiatedAt}");
            }

            var result = 0.00M;
            result += GetCashAt(inflation, at);
            result += GetValueAt(at);
            result -= GetCostAt(at);

            return result;
        }
    }
}
