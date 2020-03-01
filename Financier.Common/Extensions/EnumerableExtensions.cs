using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Financier.Common.Models;

namespace Financier.Common.Extensions
{
    public static class EnumerableExtensions
    {
        public static string Join<T>(this IEnumerable<T> enumerable, string seperator = ", ")
        {
            var sb = new StringBuilder();

            try
            {
                sb.Append(enumerable.First());
            }
            catch (InvalidOperationException)
            {
                return string.Empty;
            }

            foreach (var item in enumerable.Skip(1))
            {
                sb.Append($"{seperator}{item}");
            }

            return sb.ToString();
        }

        public static IEnumerable<T> Reject<T>(this IEnumerable<T> self, Func<T, bool> filter)
        {
            return self.Where(item => !filter(item));
        }

        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> self, Action<T> execute)
        {
            foreach (var item in self)
            {
                execute(item);
                yield return item;
            }
        }

        public static bool Empty<T>(this IEnumerable<T> self)
        {
            return !self.Any();
        }

        public static decimal TotalInflatedValue<T>(this IEnumerable<T> self, IInflation inflation, DateTime at) where T : Money
        {
            return self
                .Select(item => item.GetValueAt(inflation, at))
                .Select(adjusted => adjusted.Value)
                .Sum();
        }
    }
}
