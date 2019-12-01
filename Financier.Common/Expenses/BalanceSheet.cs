using System;
using System.Text;

namespace Financier.Common.Expenses
{
    public class BalanceSheet
    {
        public decimal Cash { get; }
        public decimal Debt { get; }
        public DateTime At { get; }

        public BalanceSheet(decimal cash, decimal debt, DateTime at)
        {
            Cash = cash;
            Debt = debt;
            At = at;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"Balance Sheet (as of {At.ToString("D")})");
            sb.AppendLine($"Cash:\t{Cash.ToString("#0.00")}");
            sb.AppendLine($"Debt:\t{Debt.ToString("#0.00")}");

            return sb.ToString();
        }
    }
}
