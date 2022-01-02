namespace Financier.Common.Expenses
{
    public class PredictionMonthlyListing : IMonthlyListing
    {
        public bool IsPrediction => true;
        public bool IsNull => false;

        public int Year { get; init; }
        public int Month { get; init; }

        public decimal Credit { get; init; }
        public decimal Debit { get; init; }
    }
}
