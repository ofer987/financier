namespace Financier.Common.Expenses
{
    public class MonthlyListing
    {
        public int Year { get; init; }
        public int Month { get; init; }

        public decimal Credit { get; init; }
        public decimal Debit { get; init; }

        public decimal Profit => Credit - Debit;
    }
}
