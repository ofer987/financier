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
                for (; datum.AddMonths(i) <= target; i += 1) ;

                return i - 1;
            }
            else if (target == datum)
            {
                return 0;
            }
            else
            {
                // Target is in the past
                // Return 0 if target is the exact same date
                var i = 0;
                for (; target.AddMonths(i) < datum; i += 1) ;

                return 1 - i;
            }
        }

        public static int SubtractMonths(this DateTime source, DateTime target)
        {
            Func<DateTime, DateTime, int> countMonths = (start, end) =>
            {
                var months = 0;
                for (; start.AddMonths(months) < end; months += 1)
                {
                }

                if (true
                    && start.AddMonths(months) < end
                    && start.AddMonths(months).Day > end.Day)
                {
                    return months + 1;
                }
                return months;
            };

            // source is in the future
            if (source > target)
            {
                return countMonths(target, source);
            }
            else if (source == target)
            {
                return 0;
            }
            else
            {
                return 0 - countMonths(source, target);
            }
        }


        public static int DaysFromBeginningOfYear(this DateTime target)
        {
            var beginningOfYear = new DateTime(target.Year, 1, 1);

            return target.Subtract(beginningOfYear).Days;
        }

        public static DateTime GetNext(this DateTime self)
        {
            return self.AddMonths(1);
        }

        public static DateTime GetPrevious(this DateTime self)
        {
            return self.AddMonths(-1);
        }

        public static bool EqualsDate(this DateTime self, DateTime target)
        {
            return true
                && self.Year == target.Year
                && self.Month == target.Month
                && self.Day == target.Day;
        }

        public static DateTime GetDate(this DateTime self)
        {
            if (self.Day > 28)
            {
                return new DateTime(
                    self.Year,
                    self.Month,
                    28
                );
            }

            return new DateTime(
                self.Year,
                self.Month,
                self.Day
            );
        }
    }
}
