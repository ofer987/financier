using System;
using System.Linq;

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

            if (DoesItemExist(record, statementId))
            {
                return null;
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

        public bool DoesItemExist(BankStatementRecord record, Guid statementId)
        {
            using (var db = new Context())
            {
                return (from items in db.Items
                        where items.StatementId == statementId
                        && items.Description == record.Description
                        && items.Amount == 0.00M - Convert.ToDecimal(record.Amount)
                        && items.TransactedAt == ToDateTime(record.PostedAt)
                        && items.PostedAt == ToDateTime(record.PostedAt)
                        select items).Any();
            }
        }
    }
}
