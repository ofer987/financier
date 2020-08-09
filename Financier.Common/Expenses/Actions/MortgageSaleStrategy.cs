using System.Linq;
using System.Collections.Generic;

namespace Financier.Common.Expenses.Actions
{
    public class MortgageSaleStrategy : ISaleStrategy
    {
        public decimal RemainingMortgageBalance { get; }
        public bool WillMortgageBeTransferred { get; }

        public MortgageSaleStrategy(decimal remainingMortgageBalance, bool willMortgageBeTransferred = false)
        {
            RemainingMortgageBalance = remainingMortgageBalance;
            WillMortgageBeTransferred = willMortgageBeTransferred;
        }

        public decimal GetReturnedPrice()
        {
            return RemainingMortgageBalance + GetFees().Sum();
        }

        public IEnumerable<decimal> GetFees()
        {
            return Enumerable.Empty<decimal>()
                .Concat(GetBankFees());
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
