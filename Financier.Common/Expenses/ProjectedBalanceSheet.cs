using System;

namespace Financier.Common.Expenses
{
    public class ProjectedBalanceSheet : BalanceSheet
    {
        public ICashFlow CashFlow { get; }

        public ProjectedBalanceSheet(ICashFlow cashFlow, decimal cash, decimal debt) : base(cash, debt, DateTime.Now)
        {
            CashFlow = cashFlow;
        }

        public ProjectedBalanceSheet(ICashFlow cashFlow, decimal cash, decimal debt, DateTime at) : base(cash, debt, at)
        {
            CashFlow = cashFlow;
        }

        public BalanceSheet GetProjectionAt(DateTime projectedAt)
        {
            if (projectedAt <= At)
            {
                throw new ArgumentOutOfRangeException(nameof(projectedAt), $"Should be later than ({At})");
            }

            var futureCash = InitialCash + CashFlow.DailyProfit * projectedAt.Subtract(At).Days;
            return new BalanceSheet(futureCash, InitialDebt, projectedAt);
        }
    }
}
