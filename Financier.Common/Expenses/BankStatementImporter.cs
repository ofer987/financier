using System;

using Financier.Common.Expenses.Models;

namespace Financier.Common.Expenses
{
    public class BankStatementImporter : StatementImporter<BankStatementRecord>
    {
        public override Item CreateItem(BankStatementRecord record, Guid statementId)
        {
            if (!record.IsValid())
            {
                throw new ArgumentException("Record is not valid");
            }

            using (var db = new Context())
            {
                var newItem = new Item
                {
                    Id = Guid.NewGuid(),
                    StatementId = statementId,
                    ItemId = Guid.NewGuid().ToString(),
                    Description = record.Description,
                    // Credit values appear as increases to account, while
                    // debits appear
                    Amount = 0.00M - Convert.ToDecimal(record.Amount),
                    TransactedAt = ToDateTime(record.PostedAt),
                    PostedAt = ToDateTime(record.PostedAt)
                };
                db.Items.Add(newItem);
                db.SaveChanges();

                return newItem;
            }
        }
    }
}
