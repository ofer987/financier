using System;

namespace Financier.Common.Extensions
{
    public static class DateTimeExtensions
    {
        public static int SubtractWholeMonths(this DateTime target, DateTime datum)
        {
            if (target > datum)
            {
                // Target is in the future
                var i = 0;
                for (; datum.AddMonths(i) < target; i += 1);

                return i;
            }
            else
            {
                // Target is in the past
                // Return 0 if target is the exact same date
                var i = 0;
                for (; target.AddMonths(i) < datum; i += 1);

                return 0 - i;
            }
        }

        public static int DaysFromBeginningOfYear(this DateTime target)
        {
            var beginningOfYear = new DateTime(target.Year, 1, 1);

            return target.Subtract(beginningOfYear).Days;
        }
    }
}
