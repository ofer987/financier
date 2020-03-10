using System;
using System.Linq;

using Financier.Common;
using Financier.Common.Models;
using Financier.Common.Expenses;
using Financier.Common.Expenses.BalanceSheets;
using Financier.Common.Expenses.Actions;
using Financier.Common.Expenses.Models;

// TODO: rename to Real Estate?
namespace Financier.Cli.BalanceSheets
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Context.Environment = Environments.Dev;

            var cashFlow = GetCashFlow();
            var startAt = cashFlow.StartAt;

            var activity = BuyOneHouse(cashFlow, startAt);
            var consumerPriceIndex = new CompoundYearlyInflation(0.02M);
            var atTwentyYears = startAt.AddYears(20);
            var netWorth = activity.GetNetWorth(consumerPriceIndex, atTwentyYears);
            var cash = activity.GetCash(consumerPriceIndex, atTwentyYears);

            var condoPurchase = activity.GetHistories()
                .Where(action => action.Type == Types.Purchase)
                .Where(action => action.Product.GetType() == typeof(Home))
                .First();

            Console.WriteLine(startAt);
            Console.WriteLine($"Daily profit: {cashFlow.DailyProfit}");
            Console.WriteLine($"One condo (purchased at {condoPurchase.At}) after 20 years");
            Console.WriteLine($"- Net Worth:\t{netWorth}");
            Console.WriteLine($"- Cash:\t{cash}");
        }

        private static SimpleCashFlow GetCashFlow()
        {
            var items = Item.FindExternalItems();

            var first = items
                .OrderBy(item => item.At)
                .First();
            var last = items
                .OrderBy(item => item.At)
                .Last();
            var amounts = items
                .Select(item => item.Amount)
                .Select(amount => 0.00M - amount);


            return new SimpleCashFlow(amounts, first.At, last.At);
        }

        // private static Activity GetActivity(ICashFlow cashFlow, DateTime startAt)
        // {
        //     var result = new Activity(cashFlow, startAt);
        //
        //     return result;
        // }

        private static Activity BuyOneHouse(ICashFlow cashFlow, DateTime startAt)
        {
            return new MagicEstateBuilder(cashFlow, startAt)
                .SetInitialCash(0000.00M)
                .AddCondoWithFixedRateMortgageAtDownPaymentPercentage(500000, 0.10M)
                .Build();
        }
    }
}
