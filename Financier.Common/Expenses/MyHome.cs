using System;
using System.Linq;

using Financier.Common.Extensions;
using Financier.Common.Liabilities;

namespace Financier.Common.Expenses
{
    public class MyHome
    {
        public decimal InitialCash { get; }
        public decimal InitialDebt { get; }
        public ICashFlow CashFlow { get; }
        public IMortgage Mortgage { get; }
        public DateTime StartAt => Mortgage.InitiatedAt;
        public decimal AnnualCashFlowProfit => CashFlow.DailyProfit * 365;
        // TODO: there has to be a better way to do this!
        public decimal MonthlyCashFlowProfit => CashFlow.DailyProfit * 30;

        private Payments expenses = new Payments();

        public static MyHome BuildStatementWithMortgage(IMortgage mortgage, ICashFlow cashflow, decimal initialCash, decimal initialDebt)
        {
            return new MyHome(mortgage, cashflow, initialCash, initialDebt);
        }

        public static MyHome BuildStatementWithPrepaybleMortgage(IMortgage baseMortgage, ICashFlow cashflow, decimal initialCash, decimal initialDebt)
        {
            var mortgage = CreatePrepayableMortgage(baseMortgage, cashflow);

            return new MyHome(mortgage, cashflow, initialCash, initialDebt);
        }

        private static PrepayableMortgage CreatePrepayableMortgage(IMortgage baseMortgage, ICashFlow cashflow)
        {
            var result = new PrepayableMortgage(baseMortgage);
            var mortgageBalance = decimal.MaxValue;
            var startAt = result.InitiatedAt;
            var at = startAt;
            while (mortgageBalance > 0.00M)
            {
                // TODO: verify whether 0:00:00.00 - 1 milliseconds
                // is the previous day
                var endOfMonth = new DateTime(at.Year, at.Month, 1)
                    .AddMonths(1)
                    .AddMilliseconds(-1);
                // FIXME: Is this API to retrieve the 12th month?
                if (endOfMonth.Month == 12)
                {
                    var yearlyProfit = cashflow.DailyProfit * endOfMonth.DaysFromBeginningOfYear();
                    // FIXME: Figure out correct amount
                    var prepayment = CreatePrepayment(
                        result.GetBalance(at),
                        yearlyProfit
                    );
                    result.AddPrepayment(
                        endOfMonth,
                        prepayment
                    );
                }

                mortgageBalance = result.GetBalance(endOfMonth);
                at = at.AddMonths(1);
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

        public MyHome(IMortgage mortgage, ICashFlow cashflow, decimal initialCash, decimal initialDebt)
        {
            Mortgage = mortgage;
            CashFlow = cashflow;
            InitialCash = initialCash;
            InitialDebt = initialDebt;
        }

        public decimal GetBalance(DateTime at)
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

        public decimal GetBalance(int months)
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
