using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Financier.Common.Actions;
using Financier.Common.Models;
using Financier.Common.Extensions;
using ActionTypes = Financier.Common.Actions.Types;

namespace Financier.Common.Expenses
{
    public class BalanceSheet
    {
        public decimal Cash { get; }
        public decimal Debt { get; }
        public DateTime At { get; }

        public IReadOnlyList<IAsset> Assets { get; }
        public IReadOnlyList<ILiability> Liabilities { get; }

        public IList<IAction> Actions { get; }

        public BalanceSheet(decimal cash, decimal debt, DateTime at)
        {
            Cash = cash;
            Debt = debt;
            At = at;
        }

        public BalanceSheet(decimal cash, decimal debt) : this(cash, debt, DateTime.Now)
        {
        }

        public void Purchase(IProduct product, decimal price, DateTime at)
        {
            // TODO: How should we handle repeat purchases?
            // TODO: should they be indicated by the same IProduct.Id?
            // TODO: or should they have a unique IProduct.Id
            Actions.Add(new Purchase(product, price, at));
        }

        public decimal GetValue()
        {
            var chronologicalActions = Actions
                .OrderBy(action => action.At)
                .Where(action => action.At < At);

            var total = Cash - Debt;
            foreach (var action in chronologicalActions)
            {
                switch (action.Type)
                {
                    case ActionTypes.Purchase:
                        total -= action.PriceAt(action.At);
                        break;
                    case ActionTypes.Sale:
                        total += action.PriceAt(action.At);
                        break;
                }
            }

            return total;
        }

        public void Sell(IProduct existingProduct, decimal price, DateTime at)
        {
            // TODO: create NullProduct class
            var purchase = GetPurchase(existingProduct);

            if (at <= purchase.At)
            {
                throw new Exception($"Error, cannot remove a product at ({at}) before it purchased at ({purchase.At})");
            }

            Actions.Add(new Sale(existingProduct, price, at));
        }

        public void Sell(IProduct existingProduct, DateTime at)
        {
            // TODO: create NullProduct class
            var purchase = GetPurchase(existingProduct);

            Sell(existingProduct, purchase.Price, at);
        }

        public Purchase GetPurchase(IProduct existingProduct)
        {
            var associatedActions = Actions
                .OrderBy(action => action.At)
                .Where(action => action.Product == existingProduct);

            if (associatedActions.Empty())
            {
                throw new Exception($"Error! the product ({existingProduct}) was never even purchased!");
            }

            var lastAction = associatedActions.Last();

            if (lastAction.Type == ActionTypes.Sale)
            {
                throw new Exception($"Error! the product ({existingProduct}) has already been sold at ({lastAction.At})");
            }

            return (Purchase)lastAction;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"Balance Sheet (as of {At.ToString("D")})");
            sb.AppendLine($"Cash:\t{Cash.ToString("#0.00")}");
            sb.AppendLine($"Debt:\t{Debt.ToString("#0.00")}");

            return sb.ToString();
        }
    }
}
