using System;
using System.Linq;
using System.Collections.Generic;

using Financier.Common.Actions;

namespace Financier.Common.Models
{
    public class BalanceSheet
    {
        public List<IAsset> Assets { get; }

        public List<ILiability> Liabilities { get; }

        public List<IAction> Actions { get; }

        public List<IProduct> Products { get; }

        public decimal Cash { get; }

        public decimal InceptionCash { get; }

        public DateTime InceptionAt { get; }

        public BalanceSheet()
        {
        }

        // public BalanceSheet GetBalanceSheet(DateTime at)
        // {
        //     // TODO should at be verified compared to InceptionAt????
        //     var cash = InceptionCash;
        //     var sortedActions = Actions
        //         .Where(action => action.Product.PurchasedAt <= at)
        //         .OrderBy(action => action.At);
        //
        //     var sortedActionsByProduct = Actions.GroupBy(
        //         action => action.Product,
        //         (product, actions) => new 
        //         {
        //             Key = product,
        //             Values = actions.OrderBy(action => action.At)
        //         }
        //     );
        //     foreach (var productActions in sortedActionsByProduct)
        //     {
        //         foreach (var action in productActions.Values)
        //         {
        //             cash -= action.Product.PurchasePrice;
        //             var valueBy = action.Product.ValueBy(at);
        //             var costBy = action.Product.CostBy(at);
        //         }
        //         actions.Values
        //     }
        // }

        public BalanceSheet(IEnumerable<IProduct> products, DateTime at)
        {
            // TODO should at be verified compared to InceptionAt????
            var assets = new List<IProduct>();
            var liabilities = new List<IProduct>();
            var cash = InceptionCash;

            var sortedSoldProducts = Products
                .Where(product => product.IsSold)
                .Where(product => product.PurchasedAt <= at)
                .Where(product => product.SoldAt <= at)
                .OrderBy(product => product.PurchasedAt);

            var sortedUnsoldProducts = Products
                .Where(product => !product.IsSold)
                .Where(product => product.PurchasedAt <= at)
                .OrderBy(product => product.PurchasedAt);

            var sortedYetToBeSoldProducts = Products
                .Where(product => product.IsSold)
                .Where(product => product.PurchasedAt <= at)
                .Where(product => product.SoldAt > at)
                .OrderBy(product => product.PurchasedAt);

            // TODO Figure out how to factor in inflation!
            foreach (var product in sortedSoldProducts)
            {
                cash -= product.PurchasePrice;
                cash += product.Sell(product.SoldAt);
            }

            foreach (var product in sortedUnsoldProducts.Concat(sortedYetToBeSoldProducts))
            {
                cash -= product.PurchasePrice;
                if (product is IAsset)
                {
                    assets.Add(product);
                }
                if (product is ILiability)
                {
                    liabilities.Add(product);
                }
            }

            Assets = assets;
            Liabilities = liabilities;
            Cash = cash;
        }

        public decimal ValueBy(int monthAfterInception)
        {
            if (monthAfterInception < 0)
            {
                throw new Exception($"{nameof(monthAfterInception)} cannot be negative number");
            }

            var totalAssetValue = Assets
                .Select(asset => asset.ValueBy(monthAfterInception))
                .Aggregate(0.00M, (result, val) => result += val);

            var totalExpenseCost = Liabilities
                .Select(liability => liability.CostBy(monthAfterInception))
                .Aggregate(0.00M, (result, val) => result += val);

            return totalAssetValue - totalExpenseCost;
        }
    }
}
