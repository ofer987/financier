using System;

namespace Financier.Common.Expenses
{
    public class ProjectedBalanceSheet : BalanceSheet
    {
        public ICashFlow CashFlow { get; }

        public ProjectedBalanceSheet(ICashFlow cashflow, decimal cash, decimal debt) : base(cash, debt, DateTime.Now)
        {
            CashFlow = cashflow;
        }

        public ProjectedBalanceSheet(ICashFlow cashflow, decimal cash, decimal debt, DateTime at) : base(cash, debt, at)
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
