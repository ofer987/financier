using System;
using System.Linq;

using Financier.Common.Liabilities;

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

        public static MyHome Process(IMortgage baseMortgage, ICashFlow cashflow, decimal initialCash, decimal initialDebt, DateTime startAt)
        {
            var mortgage = CreateMortgage(baseMortgage, cashflow, startAt);

            return new MyHome(mortgage, cashflow, initialCash, initialDebt, startAt);
        }

        private static PrepayableMortgage CreateMortgage(IMortgage baseMortgage, ICashFlow cashFlow, DateTime startAt)
        {
            var result = new PrepayableMortgage(baseMortgage, startAt);
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
                if (startAt.AddMonths(month).Month % 12 == 0)
                {
                    // FIXME: Figure out correct amount
                    var prepayment = CreatePrepayment(
                        result.GetBalance(month),
                        cashFlow.DailyProfit * 365
                    );
                    var prepayedAt = new DateTime(
                        startAt.AddMonths(month).Year,
                        startAt.AddMonths(month).Month,
                        1
                    ).AddMonths(1).AddMilliseconds(-1);
                    result.AddPrepayment(
                        prepayedAt,
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

        public MyHome(PrepayableMortgage mortgage, ICashFlow cashFlow, decimal initialCash, decimal initialDebt, DateTime startAt) : base(initialCash, initialDebt, startAt)
        {
            Mortgage = mortgage;
            CashFlow = cashFlow;
        }

        public override decimal GetBalance(DateTime at)
        {
            if (at <= StartAt)
            {
                throw new ArgumentOutOfRangeException(nameof(at), $"Value should be later than {StartAt}");
            }

            var result = 0.00M
                + Cash
                - Debt;

            result += CashFlow.DailyProfit * (at - StartAt).Days;
            result -= Mortgage.GetPrepayments(StartAt, at)
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
