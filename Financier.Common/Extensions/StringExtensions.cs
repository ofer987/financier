namespace Financier.Common.Extensions
{
    public static class StringExtensions
    {
        public static bool IsNullOrEmpty(this string self)
        {
            return string.IsNullOrEmpty(self);
        }

        public static bool IsNullOrWhiteSpace(this string self)
        {
            return string.IsNullOrWhiteSpace(self);
        }
    }
}
