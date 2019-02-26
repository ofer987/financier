using CsvHelper.Configuration.Attributes;

namespace Financier.Common.Expenses
{
    // TODO: Trim values and set Valid function
    public class CreditCardStatementRecord
    {
        [Name("Item #")]
        public string ItemId { get; set; }

        [Name("Card #")]
        public string CardNumber { get; set; }

        [Name("Transaction Date")]
        public string TransactedAt { get; set; }

        [Name("Posting Date")]
        public string PostedAt { get; set; }

        [Name("Transaction Amount")]
        public string Amount { get; set; }

        [Name("Description")]
        public string Description { get; set; }

        public override string ToString()
        {
            return $"{nameof(ItemId)}: ({ItemId})\n{nameof(CardNumber)}: ({CardNumber})\n{nameof(TransactedAt)}: ({TransactedAt})\n{nameof(PostedAt)}: ({PostedAt})\n{nameof(Amount)}: ({Amount})\n{nameof(Description)}: ({Description})";
        }
    }
}
