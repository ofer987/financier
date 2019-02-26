using System;

using Financier.Common.Expenses.Models;
using Financier.Common.Extensions;

namespace Financier.Common.Expenses
{
    public class CreditCardStatementImporter : StatementImporter<CreditCardStatementRecord>
    {
        public override Item CreateItem(CreditCardStatementRecord record, Guid statementId)
        {
            if (record.ItemId.Trim().IsNullOrEmpty())
            {
                throw new ArgumentException("cannot be blank or whitespace", nameof(record.ItemId));
            }

            using (var db = new Context())
            {
                var newItem = new Item
                {
                    Id = Guid.NewGuid(),
                    StatementId = statementId,
                    ItemId = record.ItemId.Trim(),
                    Description = record.Description,
                    Amount = Convert.ToDecimal(record.Amount),
                    TransactedAt = ToDateTime(record.TransactedAt),
                    PostedAt = ToDateTime(record.PostedAt)
                };
                db.Items.Add(newItem);
                db.SaveChanges();

                return newItem;
            }
        }
    }
}
