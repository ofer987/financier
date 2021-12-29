namespace Financier.Common.Expenses
{
    public class MonthlyListing
    {
        public int Year { get; init; }
        public int Month { get; init; }

        public decimal CreditAmount { get; init; }
        public decimal DebitAmount { get; init; }

        public decimal Profit => CreditAmount - DebitAmount;
    }
}
