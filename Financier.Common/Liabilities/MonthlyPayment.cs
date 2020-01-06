using System;
using System.Text;

namespace Financier.Common.Liabilities
{
    public class MonthlyPayment
    {
        public decimal Amount => Interest + Principal;
        public decimal Interest { get; }
        public decimal Principal { get; }
        public decimal Balance { get; }
        public DateTime At { get; }

        private IMortgage Mortgage { get; }

        public MonthlyPayment(IMortgage mortgage, DateTime at, decimal previousBalance, decimal interest, decimal principal)
        {
            Mortgage = mortgage;
            At = at;
            Interest = interest;
            Principal = principal;

            Balance = previousBalance - Principal;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"{nameof(At)}: ({At})");
            sb.AppendLine($"{nameof(Amount)}: ({Amount})");
            sb.AppendLine($"{nameof(Interest)}: ({Interest})");
            sb.AppendLine($"{nameof(Principal)}: ({Principal})");
            sb.AppendLine($"{nameof(Balance)}: {Balance}");

            return sb.ToString();
        }
    }
}
