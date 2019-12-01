using System;

namespace Financier.Common.Expenses
{
    public class ProjectedBalanceSheet : BalanceSheet
    {
        public CashFlow CashFlow { get; }

        public ProjectedBalanceSheet(CashFlow cashflow, decimal cash, decimal debt) : base(cash, debt, DateTime.Now)
        {
            CashFlow = cashflow;
        }

        public BalanceSheet GetProjectionAt(DateTime projectedAt)
        {
            if (projectedAt <= At)
            {
                throw new ArgumentOutOfRangeException(nameof(projectedAt), $"Should be later than ({At})");
            }

            var futureCash = Cash + CashFlow.DailyProfit * projectedAt.Subtract(At).Days;
            return new BalanceSheet(futureCash, Debt, projectedAt);
        }
    }
}
