using System;

using Financier.Common.Liabilities;
using Financier.Common.Models;

namespace Financier.Common.Expenses
{
    public class MyHome : BalanceSheet
    {
        public ICashFlow CashFlow { get; }
        public PrepayableMortgage Mortgage { get; }
        public DateTime StartAt => At;
        public decimal AnnualCashFlowProfit => CashFlow.DailyProfit * 365;
        // TODO: there has to be a better way to do this!
        public decimal MonthlyCashFlowProfit => CashFlow.DailyProfit * 30;

        public static MyHome Calculate(ICashFlow cashflow, IMortgage baseMortgage, DateTime startAt, decimal cash, decimal debt)
        {
            Mortgage = mortgage;
            CashFlow = cashflow;
        }

        // public MyHome(ICashFlow cashflow, decimal cash, decimal debt, DateTime at) : base(cash, debt, at)
        // {
        //     CashFlow = cashflow;
        //     if (AnnualCashFlowProfit <= 0)
        //     {
        //         return;
        //     }
        //
        //     var year = 0;
        //     int paymentsCount;
        //     do
        //     {
        //         var balance = Mortgage.GetBalance(year * 12);
        //         if (balance > AnnualCashFlowProfit)
        //         {
        //             Mortgage.AddPrepayment(at, AnnualCashFlowProfit);
        //         }
        //
        //         paymentsCount = Mortgage.GetMonthlyInterestPayments().Count();
        //     } while (paymentsCount > year * 12);
        // }

        public override decimal GetBalance(DateTime at)
        {
            if (at <= StartAt)
            {
                throw new ArgumentOutOfRangeException(nameof(at), $"Value should be later than {StartAt}");
            }

            throw new NotImplementedException("will be implemented later");
        }

        private static PrepayableMortgage GetMortgage(Home home, IMortgage baseMortgage, ICashFlow cashFlow, DateTime startAt)
        {
            var result = new PrepayableMortgage(home, baseMortgage, startAt);
            var mortgageBalance = decimal.MaxValue;
            var month = 0;
            while (mortgageBalance > 0.00M)
            {
                // FIXME: Is this API to retrieve the 12th month?
                if (startAt.AddMonths(month).Month % 12 == 0)
                {
                    // Figure out correct amount
                    var prepayment = CreatePrepayment(startAt.AddMonths(month), cashFlow.DailyProfit * 365);
                    result.AddPrepayment(startAt.AddMonths(month), prepayment);
                }

                mortgageBalance = result.GetBalance(month);
            }

            return result;
        }

        private static decimal CreatePrepayment(DateTime at, decimal annualProfit)
        {
            return annualProfit;
            // var balance = Mortgage.GetBalance(at.Year);
            // if (balance > AnnualCashFlowProfit)
            // {
            //     Mortgage.AddPrepayment(at, AnnualCashFlowProfit);
            // }
        }

        // public decimal GetBalance(int months)
        // {
        //     if (months < Mortgage.GetMonthlyInterestPayments().Count())
        //     {
        //         return 0.00M;
        //     }
        //
        //     if (months == Mortgage.GetMonthlyInterestPayments().Count())
        //     {
        //         return 0.00M;
        //     }
        //
        //     return (months - Mortgage.GetMonthlyInterestPayments().Count()) * (Convert.ToDecimal(Mortgage.MonthlyPayment) + CashFlow.DailyProfit * 30);
        // }
    }
}
