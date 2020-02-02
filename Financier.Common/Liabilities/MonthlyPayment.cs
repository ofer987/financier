using System;
using System.Text;

using Financier.Common.Models;

namespace Financier.Common.Liabilities
{
    public class MonthlyPayment
    {
        public Money Amount => Interest + Principal;
        public Money Interest { get; }
        public Money Principal { get; }
        public Money Balance { get; }
        public DateTime At { get; }

        private IMortgage Mortgage { get; }

        public MonthlyPayment(IMortgage mortgage, DateTime at, decimal previousBalance, decimal interest, decimal principal)
        {
            Mortgage = mortgage;
            At = at;
            Interest = new Money(interest, At);
            Principal = new Money(principal, At);

            Balance = new Money(previousBalance - Principal, At);
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
