using System.Collections.Generic;

namespace Financier.Common.Expenses.Actions
{
    public interface IPurchaseStrategy
    {
        IEnumerable<decimal> GetReturnedPrice();
    }
}
