using Financier.Common.Models;

namespace Financier.Common.Expenses.Actions
{
    public interface IPurchaseStrategy
    {
        Money GetReturnedPrice();
    }
}
