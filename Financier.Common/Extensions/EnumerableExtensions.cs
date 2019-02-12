using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    }
}
