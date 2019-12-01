using System;
using System.Text;

namespace Financier.Common.Expenses
{
    public class BalanceSheet
    {
        public decimal Cash { get; }
        public decimal Debts { get; }
        public DateTime At { get; }

        public BalanceSheet(decimal cash, decimal debts, DateTime at)
        {
            Cash = cash;
            Debts = debts;
            At = at;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"Balance Sheet (as of {At.ToString("D")})");
            sb.AppendLine($"Cash:\t{Cash.ToString("#0.00")}");
            sb.AppendLine($"Debts:\t{Debts.ToString("#0.00")}");

            return sb.ToString();
        }
    }
}
