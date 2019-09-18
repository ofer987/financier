using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Financier.Common;
using Financier.Common.Extensions;
using Financier.Common.Expenses;

namespace Financier.Web.ViewModels
{
    public class MonthlyExpenses
    {
        public DateTime EarliestAt
        {
            get
            {
                DateTime date;
                using (var db = new Context())
                {
                    date = db.Items
                        .OrderBy(item => item.TransactedAt)
                        .First()
                        .TransactedAt;
                }

                return new DateTime(date.Year, date.Month, 1);
            }
        }

        public DateTime LatestAt
        {
            get
            {
                DateTime date;
                using (var db = new Context())
                {
                    date = db.Items
                        .OrderByDescending(item => item.TransactedAt)
                        .First()
                        .TransactedAt;
                }

                return new DateTime(date.Year, date.Month, 1).AddMonths(1).AddDays(-1);
            }
        }

        public IEnumerable<DateTime> Months
        {
            get
            {
                var earliestAt = EarliestAt;
                var latestAt = LatestAt;

                for (var startAt = earliestAt; startAt <= latestAt; startAt = startAt.AddMonths(1))
                {
                    yield return startAt;
                }
            }
        }

        public int Year { get; }
        public int Month { get; }
        public MonthlyStatement Statement { get; }
        public DateTime From { get; }
        public DateTime To { get; }

        public MonthlyExpenses(int year, int month, MonthlyStatement statement)
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

            Statement = statement;
        }

        public IEnumerable<Financier.Common.Expenses.Models.Item> GetItems()
        {
            return Financier.Common.Expenses.Models.Item.FindDebits(From, To);
        }
    }
}
