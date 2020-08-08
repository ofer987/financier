using System.Linq;
using System.Collections.Generic;

namespace Financier.Common.Expenses.Actions
{
    public class HomeSaleStrategy : ISaleStrategy
    {
        public decimal RequestedPrice { get; }
        public decimal RemainingMortgageBalance { get; }
        public decimal AvailableCash { get; }
        public bool WillMortgageBeTransferred { get; }

        public HomeSaleStrategy(decimal requestedPrice, decimal remainingMortgageBalance, bool willMortgageBeTransferred = false)
        {
            RequestedPrice = requestedPrice;
            RemainingMortgageBalance = remainingMortgageBalance;
            WillMortgageBeTransferred = willMortgageBeTransferred;
        }

        public decimal GetReturnedPrice()
        {
            return RequestedPrice - GetFees().Sum();
        }

        public IEnumerable<decimal> GetFees()
        {
            return Enumerable.Empty<decimal>()
                .Concat(GetRealtorFees())
                .Concat(GetBankFees());
        }

        public IEnumerable<decimal> GetRealtorFees()
        {
            yield return 0.05M * RequestedPrice;
        }

        public IEnumerable<decimal> GetLegalFees()
        {
            yield return 1000.00M;
        }

        public IEnumerable<decimal> GetBankFees()
        {
            if (!WillMortgageBeTransferred)
            {
                yield return 0.05M * RemainingMortgageBalance;
            }
        }
    }
}
