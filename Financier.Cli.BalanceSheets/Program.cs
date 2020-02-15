using System;

using Financier.Common.Models;
using Financier.Common.Expenses;

namespace Financier.Cli.BalanceSheets
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }

        private BalanceSheet GetSequentialBalanceSheet()
        {
            var initiatedAt = new DateTime(2019, 1, 1);
            var zeroDollars = new Money(0.00M, initiatedAt);

            return new BalanceSheet(zeroDollars, zeroDollars, new DummyCashFlow(0.00M), initiatedAt)
                .AddHome(new Home);
        }
    }
}
