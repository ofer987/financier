using System.Collections.Generic;
using System.Linq;

using Financier.Common.Expenses.Models;

namespace Financier.Common.Extensions
{
    public static class TagExtensions
    {
        public static bool HasCreditCardPayment(this IEnumerable<Tag> tags)
        {
            return tags.Any(tag => tag.Name == "credit-card-payment");
        }

        public static bool HasInternalTransfer(this IEnumerable<Tag> tags)
        {
            return tags.Any(tag => tag.IsInternalTransfer());
        }

        public static bool IsInternalTransfer(this Tag tag)
        {
            return false
                || tag.Name == "transfer"
                || tag.Name == "internal"
                || tag.Name == "investments";
        }
    }
}
