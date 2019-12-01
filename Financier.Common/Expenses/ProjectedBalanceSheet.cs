using System;

namespace Financier.Common.Expenses
{
    public class ProjectedBalanceSheet : BalanceSheet
    {
        public CashFlow CashFlow { get; }

        public ProjectedBalanceSheet(CashFlow cashflow, decimal cash, decimal debts) : base(cash, debts, DateTime.Now)
        {
            CashFlow = cashflow;
        }

        public BalanceSheet GetProjectionAt(DateTime projectAt)
        {
            if (projectAt <= At)
            {
                throw new ArgumentOutOfRangeException(nameof(projectAt), $"Should be later than ({At})");
            }

            var futureCash = Cash + CashFlow.DailyProfit * projectAt.Subtract(At).Days;
            return new BalanceSheet(futureCash, Debts, projectAt);
        }
    }
}
