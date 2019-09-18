using System.Collections.Generic;
using System.Linq;

using Financier.Common.Expenses.Models;

namespace Financier.Common.Extensions
{
    public static class TagExtensions
    {
        public static bool HasCreditCardPayent(this IEnumerable<Tag> tags)
        {
            return tags.Any(tag => tag.Name == "credit-card-payment");
        }

        public static bool HasInternalTransfer(this IEnumerable<Tag> tags)
        {
            return tags.Any(tag => tag.Name == "transfer");
        }
    }
}
