namespace Financier.Common.Expenses
{
    public class MonthlyListing : IMonthlyListing
    {
        public bool IsPrediction { get; init; }

        public int Year { get; init; }
        public int Month { get; init; }

        public decimal Credit { get; init; }
        public decimal Debit { get; init; }

        public bool IsNull => false;
    }
}
