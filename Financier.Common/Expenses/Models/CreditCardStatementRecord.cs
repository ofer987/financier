using System;
using CsvHelper.Configuration.Attributes;

namespace Financier.Common.Expenses.Models
{
    // TODO: Trim values and set Valid function
    public class CreditCardStatementRecord : StatementRecord
    {
        public override CardTypes CardType => CardTypes.Credit;

        [Ignore]
        public override string AccountName { get; init; }

        [Name("Item #")]
        public override string ItemId { get; set; }

        [Name("Card #")]
        public override string Number { get; set; }

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
            return $"{nameof(ItemId)}: ({ItemId})\n{nameof(Number)}: ({Number})\n{nameof(TransactedAt)}: ({TransactedAt})\n{nameof(PostedAt)}: ({PostedAt})\n{nameof(Amount)}: ({Amount})\n{nameof(Description)}: ({Description})";
        }

        public override Item CreateItem(Guid statementId)
        {
            Validate();

            var amount = Convert.ToDecimal(Amount);
            using (var db = new Context())
            {
                var newItem = new Item(
                    id: Guid.NewGuid(),
                    statementId: statementId,
                    itemId: ItemId.Trim(),
                    description: Description,
                    transactedAt: ToDateTime(TransactedAt),
                    postedAt: ToDateTime(PostedAt),
                    amount: amount
                );
                db.Items.Add(newItem);
                db.SaveChanges();

                return newItem;
            }
        }
    }
}
