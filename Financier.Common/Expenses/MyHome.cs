using System;

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

        public MyHome(ICashFlow cashflow, PrepayableMortgage mortgage, DateTime startAt, decimal cash, decimal debt) : base(cash, debt, startAt)
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

        public override decimal GetBalance(int months)
        {
            if (months <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(months), "Value should be greater than 0");
            }

            var mortgageBalance = decimal.MaxValue;
            int i;
            var result = 0.00M;
            for (i = 0; mortgageBalance > 0.00M && i <= months; i += 1)
            {
                result = 0.00M
                    + Cash
                    - Debt;

                // FIXME: Is this API to retrieve the 12th month?
                if (StartAt.AddMonths(i).Month == 12)
                {
                    AddPrepayment(StartAt.AddMonths(i));
                }

                mortgageBalance = Mortgage.GetBalance(i);
                result += mortgageBalance;
            }
            result += MonthlyCashFlowProfit * (months - i);

            return decimal.Round(result, 2);
        }

        private void AddPrepayment(DateTime at)
        {
            var balance = Mortgage.GetBalance(at.Year);
            if (balance > AnnualCashFlowProfit)
            {
                Mortgage.AddPrepayment(at, AnnualCashFlowProfit);
            }
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
