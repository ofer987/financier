using System;

using Financier.Common.Models;
using Financier.Common.Extensions;

namespace Financier.Common.Expenses.BalanceSheets
{
    public class CashFinder
    {
        public DateTime InitiatedAt { get; }
        public ICashFlow CashFlow { get; }

        public CashFinder(ICashFlow cashFlow, DateTime initiatedAt)
        {
            CashFlow = cashFlow;
            InitiatedAt = initiatedAt;
        }

        public DateTime HasAvailableCashAt(decimal expenseAtInitiation, IInflation expenseInflation)
        {
            var atFiftyYears = InitiatedAt.AddYears(50);
            DateTime at = InitiatedAt;
            decimal available, required;

            do
            {
                if (at > atFiftyYears)
                {
                    throw new InvalidOperationException($"Unable to find enough money to pay for {expenseAtInitiation} by ${atFiftyYears.ToString("yyyy-MM-dd")}");
                }

                at = at.GetNext();
                available = CashFlow.GetCash(expenseInflation, InitiatedAt, at);
                required = CostAt(expenseAtInitiation, expenseInflation, at);

            } while (available < required);

            return at;
        }

        private decimal CostAt(decimal expenseAtInitiation, IInflation expenseInflation, DateTime at)
        {
            return new Money(
                expenseAtInitiation,
                InitiatedAt
            ).GetValueAt(
                expenseInflation,
                at
            ).Value;
        }
    }
}
