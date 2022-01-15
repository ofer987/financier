namespace Financier.Common.Expenses
{
    public interface IMonthlyListing
    {
        bool IsPrediction { get; }
        bool IsNull { get; }

        int Year { get; init; }
        int Month { get; init; }

        decimal Credit { get; }
        decimal Debit { get; }

        decimal Profit => Credit - Debit;
    }
}
