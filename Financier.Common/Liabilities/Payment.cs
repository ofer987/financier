using System;
using System.Text;

namespace Financier.Common.Liabilities
{
    public class Payment
    {
        public decimal Amount => Interest + Principal;
        public decimal Interest { get; private set; }
        public decimal Principal { get; private set; }
        public decimal Balance { get; private set; }
        public DateTime At { get; private set; }

        private IMortgage Mortgage { get; }

        public Payment(IMortgage mortgage, DateTime at, decimal previousBalance, decimal interest, decimal principal)
        {
            Mortgage = mortgage;
            At = at;
            Interest = decimal.Round(interest, 2);
            Principal = decimal.Round(principal, 2);

            Balance = decimal.Round(previousBalance - Principal, 2);
        }

        private Payment()
        {
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
