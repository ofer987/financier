using System;
using System.Linq;

using Financier.Common.Liabilities;

namespace Financier.Common.Expenses
{
    public class MyHome : BalanceSheet
    {
        public ICashFlow CashFlow { get; }
        public PrepayableMortgage Mortgage { get; }
        public decimal AnnualCashFlowProfit => CashFlow.DailyProfit * 365;

        public MyHome(ICashFlow cashflow, decimal cash, decimal debt) : base(cash, debt, DateTime.Now)
        {
            CashFlow = cashflow;
        }

        public MyHome(ICashFlow cashflow, decimal cash, decimal debt, DateTime at) : base(cash, debt, at)
        {
            CashFlow = cashflow;
            if (AnnualCashFlowProfit <= 0)
            {
                return;
            }

            var year = 0;
            int paymentsCount;
            do
            {
                var balance = Mortgage.GetBalance(year * 12);
                if (balance > AnnualCashFlowProfit)
                {
                    Mortgage.AddPrepayment(at, AnnualCashFlowProfit);
                }

                paymentsCount = Mortgage.GetMonthlyInterestPayments().Count();
            } while (paymentsCount > year * 12);
        }

        public decimal GetBalance(int months)
        {
            if (months < Mortgage.GetMonthlyInterestPayments().Count())
            {
                return 0.00M;
            }

            if (months == Mortgage.GetMonthlyInterestPayments().Count())
            {
                return 0.00M;
            }

            return (months - Mortgage.GetMonthlyInterestPayments().Count()) * (Convert.ToDecimal(Mortgage.MonthlyPayment) + CashFlow.DailyProfit * 30);
        }
    }
}
