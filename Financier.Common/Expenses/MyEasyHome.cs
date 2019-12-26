using System;

using Financier.Common.Liabilities;

namespace Financier.Common.Expenses
{
    public class MyEasyHome : BalanceSheet
    {
        public ICashFlow CashFlow { get; }
        public Mortgage Mortgage { get; }
        public decimal AnnualCashFlowProfit => CashFlow.DailyProfit * 365;

        public MyEasyHome(ICashFlow cashflow, decimal cash, decimal debt) : base(cash, debt, DateTime.Now)
        {
            CashFlow = cashflow;
        }

        public MyEasyHome(ICashFlow cashflow, decimal cash, decimal debt, DateTime at) : base(cash, debt, at)
        {
            CashFlow = cashflow;
        }

        public decimal GetBalance(int months)
        {
            if (months < Mortgage.AmortisationPeriodInMonths)
            {
                return 0.00M;
            }

            if (months == Mortgage.AmortisationPeriodInMonths)
            {
                return 0.00M;
            }

            return (months - Mortgage.AmortisationPeriodInMonths) * (Convert.ToDecimal(Mortgage.MonthlyPayment) + CashFlow.DailyProfit * 30);
        }
    }
}
