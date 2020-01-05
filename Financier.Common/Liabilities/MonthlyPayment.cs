using System;

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
            Interest = decimal.Round(interest, 2);
            Principal = decimal.Round(principal, 2);

            Balance = previousBalance - Principal;
        }
    }
}
