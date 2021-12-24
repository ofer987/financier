using System;
using System.Linq;

using Financier.Common;
using Financier.Common.Models;
using Financier.Common.Expenses;
using Financier.Common.Expenses.BalanceSheets;
using Financier.Common.Expenses.Actions;

// TODO: rename to Real Estate?
namespace Financier.Cli.BalanceSheets
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Context.Environment = Environments.Dev;

            var cashFlow = GetCashFlow();
            var startAt = new DateTime(2018, 11, 1);
            var atTwentyYears = startAt.AddYears(20);

            Console.WriteLine(startAt);
            Console.WriteLine($"Daily profit: {cashFlow.DailyProfit}");

            var previousCash = 0.00M;
            for (var downPaymentRate = 0.05M; downPaymentRate <= 0.20M; downPaymentRate += 0.05M)
            {
                previousCash = PrintCashFlow(downPaymentRate, cashFlow, startAt, atTwentyYears, previousCash);
            }
        }

        private static decimal PrintCashFlow(decimal downPaymentRate, ICashFlow cashFlow, DateTime startedAt, DateTime soldAt, decimal previousCash)
        {
            var activity = BuyAndSellOneHouse(cashFlow, startedAt, soldAt, downPaymentRate);
            var consumerPriceIndex = new CompoundYearlyInflation(0.02M);

            // var netWorth = activity.GetNetWorth(consumerPriceIndex, atTwentyYears.AddDays(1));
            var cash = activity.GetCashAt(Inflations.NoopInflation, soldAt.AddDays(1));

            var condoPurchase = activity.GetHistories()
                .Where(action => action.Type == Types.Purchase)
                .Where(action => action.Product.GetType() == typeof(Home))
                .First();

            Console.WriteLine($"\tPurchased with {downPaymentRate * 100} per cent down at {condoPurchase.At} after 20 years");
            Console.WriteLine($"\t- Cash:\t\t{cash}");
            if (previousCash != 0.00M)
            {
                Console.WriteLine($"\t- %+- Change:\t{((cash / previousCash) - 1).ToString("#0.00 %")}");
            }

            return cash;
        }

        private static ICashFlow GetCashFlow()
        {
            return new DummyCashFlow(50.00M);
        }

        // private static Activity GetActivity(ICashFlow cashFlow, DateTime startAt)
        // {
        //     var result = new Activity(cashFlow, startAt);
        //
        //     return result;
        // }

        private static Activity BuyAndSellOneHouse(ICashFlow cashFlow, DateTime startAt, DateTime soldAt, decimal downPaymentRate)
        {
            var condoAppreciation = new CompoundYearlyInflation(0.05M);
            var soldPrice = new Money(500000, startAt)
                .GetValueAt(condoAppreciation, soldAt);

            return new MagicEstateBuilder(cashFlow, startAt)
                .SetInitialCash(50000.00M)
                .AddCondoWithFixedRateMortgageAtDownPaymentPercentage("first", 500000, downPaymentRate)
                .SellHome("first", soldAt, soldPrice)
                .Build();
        }
    }
}
