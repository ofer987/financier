using System;
using System.Linq;
using McMaster.Extensions.CommandLineUtils;

using Financier.Common;
using Financier.Common.Expenses;

namespace Financier.Cli.Listings
{
    public class Program
    {
        public static void Main(string[] args) => CommandLineApplication.Execute<Program>(args);

        private DateTime EarliestAt
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

        private DateTime LatestAt
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

        private void OnExecute()
        {
            Context.Environment = Environments.Dev;

            var earliestAt = EarliestAt;
            var latestAt = LatestAt;

            Console.WriteLine("Cash Flow");
            var startAt = earliestAt;
            while (startAt <= latestAt)
            {
                var endAt = startAt.AddMonths(1).AddDays(-1);
                var items = new Analysis(startAt, endAt).GetAllExpenses();

                var amount = items.Aggregate(0.00M, (result, item) => result + item.Amount);
                Console.WriteLine($"\t{startAt.ToString("MMMM yyyy")}\t{amount}");

                startAt = startAt.AddMonths(1);
            }

            Console.WriteLine();
            Console.WriteLine("Expenditures");
            startAt = earliestAt;
            while (startAt <= latestAt)
            {
                var endAt = startAt.AddMonths(1).AddDays(-1);

                Console.WriteLine($"\t{startAt.ToString("MMMM yyyy")}");
                DisplayItemsByTag(startAt, endAt);

                startAt = endAt;
            }
        }

        private void DisplayItemsByTag(DateTime startAt, DateTime endAt)
        {
            foreach (var result in new Analysis(startAt, endAt).GetItemsByTag().GroupBy(result => result.TagName))
            {
                var tagName = result.Key;

                var total = result.Aggregate(0.00M, (r, i) => r + i.ItemAmount);
                Console.WriteLine($"\t\t{tagName} for a total of {total}");
                foreach (var item in result)
                {
                    if (item.ItemAmount >= 0)
                    {
                        Console.WriteLine($"\t\t\t{item.ItemDescription} bought for {item.ItemAmount} dollars");
                    }
                    else
                    {
                        Console.WriteLine($"\t\t\t{item.ItemDescription} earned for {item.ItemAmount} dollars");
                    }
                }
            }
        }
    }
}
