using System;

namespace Financier.Common.Extensions
{
    public static class DateTimeExtensions
    {
        public static int SubtractWholeMonths(this DateTime datum, DateTime target)
        {
            if (datum > target)
            {
                // Target is in the past
                var i = 0;
                for (; datum.AddMonths(i) > target; i -= 1);

                return i;
            }
            else
            {
                // Target is in the future
                // Return 0 if target is the exact same date
                var i = 0;
                for (; target.AddMonths(i) < datum; i += 1);

                return i;
            }
        }
    }
}
