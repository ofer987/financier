namespace Financier.Common.Expenses
{
    public class NullMonthlyListing : IMonthlyListing
    {
        public bool IsPrediction => false;

        public int Year { get; init; }
        public int Month { get; init; }

        public decimal Credit => 0.00M;
        public decimal Debit => 0.00M;

        public bool IsNull => true;
    }
}
