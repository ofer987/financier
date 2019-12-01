using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Globalization;
using McMaster.Extensions.CommandLineUtils;

using Financier.Common;
using Financier.Common.Extensions;
using Financier.Common.Expenses;
using Financier.Common.Expenses.Models;

namespace Financier.Cli.BalanceSheet
{
    public class Program
    {
        [OptionAttribute("-a|--projected-at", CommandOptionType.SingleValue)]
        public String ProjectedAtArgument { get; }

        [OptionAttribute("-c|--cash", CommandOptionType.SingleValue)]
        public decimal Cash { get; private set; }

        [OptionAttribute("-c|--debt", CommandOptionType.SingleValue)]
        public decimal Debt { get; private set; }

        public DateTime ProjectedAt => DateTime.ParseExact(ProjectedAtArgument, "yyyyMMdd", CultureInfo.InvariantCulture);
        public DateTime At { get; private set; }

        public static int Main(string[] args) => CommandLineApplication.Execute<Program>(args);

        private void OnExecute()
        {
            Context.Environment = Environments.Dev;
            At = DateTime.Now;

            var cashFlow = new CashFlow(GetEarliestItem(), GetLatestItem());
            var projectedBalanceSheet = new ProjectedBalanceSheet(cashFlow, Cash, Debt);

            Console.WriteLine(projectedBalanceSheet);
        }

        private DateTime GetEarliestItem()
        {
            using (var db = new Context())
            {
                return db.Items
                    .OrderBy(item => item.At)
                    .First;
            }
        }

        private DateTime GetLatestItem()
        {
            using (var db = new Context())
            {
                return db.Items
                    .OrderByDescending(item => item.At)
                    .First;
            }
        }
    }
}
