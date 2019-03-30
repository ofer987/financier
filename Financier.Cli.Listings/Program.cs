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

        private void OnExecute()
        {
            Context.Environment = Environments.Dev;

            DateTime earliestAt, latestAt;
            using (var db = new Context())
            {
                earliestAt = db.Items
                    .OrderBy(item => item.TransactedAt)
                    .First()
                    .TransactedAt;

                latestAt = db.Items
                    .OrderByDescending(item => item.TransactedAt)
                    .First()
                    .TransactedAt;
            }

            earliestAt = new DateTime(earliestAt.Year, earliestAt.Month, 1);
            latestAt = new DateTime(latestAt.Year, latestAt.Month, 1).AddMonths(1).AddDays(-1);

            var startAt = earliestAt;
            while (startAt <= latestAt)
            {
                var endAt = startAt.AddMonths(1).AddDays(-1);
                var items = new Analysis(startAt, endAt).GetAllExpenses();

                var amount = items.Aggregate(0.00M, (result, item) => result + item.Amount);
                Console.WriteLine($"{startAt.ToString("MMMM yyyy")}\t{amount}");

                startAt = startAt.AddMonths(1);
            }
        }
    }
}
