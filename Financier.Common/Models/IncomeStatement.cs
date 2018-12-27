using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace Financier.Common.Models
{
    public class IncomeStatement
    {
        public List<Income.Base> IncomeSources { get; } = new List<Income.Base>();

        public List<IAsset> Assets { get; } = new List<IAsset>();

        public List<ILiability> Liabilities { get; } = new List<ILiability>();

        public decimal Cash { get; }

        public DateTime From { get; }

        public DateTime To { get; }

        public IncomeStatement(decimal inceptionCash, IEnumerable<Income.Base> incomeSources, IEnumerable<IProduct> products, DateTime from, DateTime to)
        {
            if (to < from)
            {
                throw new Exception($"Income Statement cannot be computed in reverse order from ({from}) to ({to})");
            }

            Cash = inceptionCash;
            IncomeSources = incomeSources.ToList();

            From = from;
            To = to;

            var sortedSoldProducts = products
                .Where(product => product.IsSold)
                .Where(product => product.PurchasedAt >= from)
                .Where(product => product.PurchasedAt <= to)
                .Where(product => product.SoldAt <= to)
                .OrderBy(product => product.PurchasedAt);

            var sortedUnsoldProducts = products
                .Where(product => !product.IsSold)
                .Where(product => product.PurchasedAt >= from)
                .Where(product => product.PurchasedAt <= to)
                .OrderBy(product => product.PurchasedAt);

            var sortedYetToBeSoldProducts = products
                .Where(product => product.IsSold)
                .Where(product => product.PurchasedAt >= from)
                .Where(product => product.PurchasedAt <= to)
                .Where(product => product.SoldAt > to)
                .OrderBy(product => product.PurchasedAt);

            // TODO Figure out how to factor in inflation!
            foreach (var product in sortedSoldProducts)
            {
                // Cash -= product.PurchasePrice;
                Cash += product.ValueBy(to);
                Cash -= product.CostBy(to);
            }

            foreach (var product in sortedUnsoldProducts.Concat(sortedYetToBeSoldProducts))
            {
                // Cash -= product.PurchasePrice;
                Assets.AddRange(product.Assets);
                Liabilities.AddRange(product.Liabilities);
            }
        }

        public decimal TotalValue()
        {
            var result = Cash;

            var assetValue = Assets
                .Select(asset => asset.ValueBy(To))
                .Aggregate(0.00M, (total, val) => total += val);

            var liabilityCost = Liabilities
                .Select(liability => liability.CostBy(To))
                .Aggregate(0.00M, (total, val) => total += val);

            return Cash + assetValue - liabilityCost;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            var cash = IncomeSources.Aggregate(Cash, (total, val) => total += val.Value(From, To));
            sb.AppendLine($"Cash\t\t\t\t{cash:C2}");

            sb.AppendLine("Revenue:");
            foreach (var asset in Assets)
            {
                sb.AppendLine($"\t{asset.Product.Name}\t\t{asset.ValueBy(To):C2}");
            }

            sb.AppendLine("Expenses:");
            foreach (var liability in Liabilities)
            {
                sb.AppendLine($"\t{liability.Product.Name}\t\t{liability.CostBy(To):C2}");
            }

            sb.AppendLine($"Net Worth\t\t\t{TotalValue():C2}");

            return sb.ToString();
        }
    }
}
