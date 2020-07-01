using System.Collections.Generic;

using Financier.Common.Models;

namespace Financier.Common.Expenses.Actions
{
    public interface IPurchaseStrategy
    {
        IEnumerable<Money> GetReturnedPrice();
    }
}
