using System;
using System.Linq;

using Financier.Common.Extensions;
using Financier.Common.Liabilities;

namespace Financier.Common.Expenses
{
    public class MyHome : BalanceSheet
    {
        public ICashFlow CashFlow { get; }
        public IMortgage Mortgage { get; }
        public DateTime StartAt => At;
        public decimal AnnualCashFlowProfit => CashFlow.DailyProfit * 365;
        // TODO: there has to be a better way to do this!
        public decimal MonthlyCashFlowProfit => CashFlow.DailyProfit * 30;

        private Payments expenses = new Payments();

        public static MyHome BuildStatementWithMortgage(IMortgage mortgage, ICashFlow cashflow, decimal initialCash, decimal initialDebt, DateTime startAt)
        {
            return new MyHome(mortgage, cashflow, initialCash, initialDebt, startAt);
        }

        public static MyHome BuildStatementWithPrepaybleMortgage(IMortgage baseMortgage, ICashFlow cashflow, decimal initialCash, decimal initialDebt, DateTime startAt)
        {
            var mortgage = CreatePrepayableMortgage(baseMortgage, cashflow, startAt);

            return new MyHome(mortgage, cashflow, initialCash, initialDebt, startAt);
        }

        private static PrepayableMortgage CreatePrepayableMortgage(IMortgage baseMortgage, ICashFlow cashflow, DateTime startAt)
        {
            var result = new PrepayableMortgage(baseMortgage);
            var mortgageBalance = decimal.MaxValue;
            var month = 0;
            while (mortgageBalance > 0.00M)
            {
                // TODO: verify whether 0:00:00.00 - 1 milliseconds
                // is the previous day
                var endOfMonth = new DateTime(
                    startAt.AddMonths(month).Year,
                    startAt.AddMonths(month).Month,
                    1
                ).AddMonths(1).AddMilliseconds(-1);
                // FIXME: Is this API to retrieve the 12th month?
                if (endOfMonth.Month == 12)
                {
                    var yearlyProfit = cashflow.DailyProfit * endOfMonth.DaysFromBeginningOfYear();
                    // FIXME: Figure out correct amount
                    var prepayment = CreatePrepayment(
                        result.GetBalance(month),
                        yearlyProfit
                    );
                    result.AddPrepayment(
                        endOfMonth,
                        prepayment
                    );
                }

                mortgageBalance = result.GetBalance(endOfMonth);
            }

            return result;
        }

        private static decimal CreatePrepayment(decimal balance, decimal annualProfit)
        {
            return balance > annualProfit
                ? annualProfit
                : balance;
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

        public MyHome(IMortgage mortgage, ICashFlow cashflow, decimal initialCash, decimal initialDebt, DateTime startAt) : base(initialCash, initialDebt, startAt)
        {
            Mortgage = mortgage;
            CashFlow = cashflow;
        }

        public override decimal GetBalance(DateTime at)
        {
            if (at <= StartAt)
            {
                throw new ArgumentOutOfRangeException(nameof(at), $"Value should be later than {StartAt}");
            }

            var result = 0.00M
                + InitialCash
                - InitialDebt;

            result += CashFlow.DailyProfit * (at - StartAt).Days;
            result -= expenses.GetRange(StartAt, at)
                .Select(payment => payment.Item2)
                .Sum();
            result -= Mortgage.GetBalance(at);

            return result;
        }

        public override decimal GetBalance(int months)
        {
            if (months <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(months), "Value should be greater than 0");
            }

            throw new NotImplementedException("will be implemented later");
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
