using System;

using Financier.Common.Liabilities;

namespace Financier.Common.Expenses
{
    public class MyEasyHome : BalanceSheet
    {
        public ICashFlow CashFlow { get; }
        public Mortgage Mortgage { get; }
        public DateTime StartAt => At;
        public decimal AnnualCashFlowProfit => CashFlow.DailyProfit * 365;
        // TODO: there has to be a better way to do this!
        public decimal MonthlyCashFlowProfit => CashFlow.DailyProfit * 30;

        public MyEasyHome(ICashFlow cashflow, Mortgage mortgage, DateTime startAt, decimal cash, decimal debt) : base(cash, debt, startAt)
        {
            CashFlow = cashflow;
            Mortgage = mortgage;
        }

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

            var result = 0.00M
                + Cash
                + MonthlyCashFlowProfit * months
                - Debt
                - Mortgage.GetBalance(months);
            return decimal.Round(result, 2);
        }
    }
}
