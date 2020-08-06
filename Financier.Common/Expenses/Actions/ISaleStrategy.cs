using System.Collections.Generic;

namespace Financier.Common.Expenses.Actions
{
    public interface ISaleStrategy
    {
        IEnumerable<decimal> GetReturnedPrice();
    }
}
