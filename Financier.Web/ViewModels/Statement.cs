using System;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

using Financier.Common.Extensions;
using Financier.Common.Expenses.Models;

namespace Financier.Web.ViewModels
{
    public class Statement
    {
        public int Year { get; }
        public int Month { get; }
        public DateTime At => new DateTime(Year, Month, 1);

        public DateTime From { get; }
        public DateTime To { get; }

        public Statement(int year, int month)
        {
            Year = year;
            Month = month;
            From = new DateTime(year, month, 1);
            
            if (month == 12)
            {
                To = new DateTime(year + 1, 1, 1).AddDays(-1);
            }
            else
            {
                To = new DateTime(year, month + 1, 1).AddDays(-1);
            }
        }

        private decimal? expenseTotal = null;
        public decimal GetExpenseTotal()
        {
            if (!expenseTotal.HasValue)
            {
                expenseTotal = Financier.Common.Expenses.Models.Item
                    .FindDebits(From, To)
                    .Sum(item => item.Amount);
                // return db.Items
                //     .Where(item => item.Amount > 0)
                //     .Where(item => item.TransactedAt >= From)
                //     .Where(item => item.TransactedAt < To)
                //     .Sum(item => item.Amount);
            }

            return expenseTotal.Value;
        }

        private decimal? assetTotal = null;
        public decimal GetAssetTotal()
        {
            if (!assetTotal.HasValue)
            {
                assetTotal = Financier.Common.Expenses.Models.Item
                    .FindCredits(From, To)
                    .Sum(item => item.Amount);
                // using (var db = new Context())
                // {
                //     assetTotal = 0 - db.Items
                //         .Where(item => item.Amount < 0)
                //         .Where(item => item.TransactedAt >= From)
                //         .Where(item => item.TransactedAt < To)
                //         .Sum(item => item.Amount);
                // }
            }

            return assetTotal.Value;
        }

        public decimal GetProfitTotal()
        {
            return GetAssetTotal() - GetExpenseTotal();
        }

        public IEnumerable<Financier.Common.Expenses.Models.Item> GetItems()
        {
            return Financier.Common.Expenses.Models.Item.FindExternalItems(From, To);
        }

        public IEnumerable<TagCost> GetTagCostAssets()
        {
            return new TagAnalysis(Year, Month).GetAssets();
        }

        public IEnumerable<TagCost> GetTagCostExpenses()
        {
            return new TagAnalysis(Year, Month).GetExpenses();
        }
    }
}
