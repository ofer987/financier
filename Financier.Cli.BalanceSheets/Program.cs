using System;
using System.Linq;
using System.Globalization;
using McMaster.Extensions.CommandLineUtils;

using Financier.Common;
using Financier.Common.Expenses;

namespace Financier.Cli.BalanceSheets
{
    public class Program
    {
        [OptionAttribute("-a|--projected-at", CommandOptionType.SingleValue)]
        public String ProjectedAtArgument { get; }

        [OptionAttribute("-c|--cash", CommandOptionType.SingleValue)]
        public string CashArgument { get; private set; }
        public decimal Cash => decimal.Parse(CashArgument);

        [OptionAttribute("-d|--debt", CommandOptionType.SingleValue)]
        public string DebtArgument { get; private set; }
        public decimal Debt => decimal.Parse(DebtArgument);

        public DateTime ProjectedAt => DateTime.ParseExact(ProjectedAtArgument, "yyyyMMdd", CultureInfo.InvariantCulture);
        public DateTime At { get; private set; }

        public static int Main(string[] args) => CommandLineApplication.Execute<Program>(args);

        private void OnExecute()
        {
            Context.Environment = Environments.Dev;

            var incomeStatement = new OneHome();
            Console.WriteLine(incomeStatement.GetValueAt(new DateTime(2030, 1, 1)));
        }
    }
}
