using System;
using System.Text;

using Financier.Common.Models;

namespace Financier.Common.Liabilities
{
    public class Payment
    {
        public Money Amount => Interest + Principal;
        public Money Interest { get; private set; }
        public Money Principal { get; private set; }
        public Money Balance { get; private set; }
        public DateTime At { get; private set; }

        private IMortgage Mortgage { get; }

        public Payment(IMortgage mortgage, DateTime at, decimal previousBalance, decimal interest, decimal principal)
        {
            Mortgage = mortgage;
            At = at;
            Interest = new Money(interest, At);
            Principal = new Money(principal, At);

            Balance = new Money(previousBalance - Principal.Value, At);
        }

        private Payment()
        {
        }

        // TODO: write tests against this
        public Payment GetValueAt(IInflation inflation, DateTime inflatedAt)
        {
            // TODO: how should new MonthlyPayment objects be created?
            return new Payment
            {
                At = At,
                Principal = Principal.GetValueAt(inflation, inflatedAt),
                Interest = Interest.GetValueAt(inflation, inflatedAt),
                Balance = Balance.GetValueAt(inflation, inflatedAt)
            };
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
