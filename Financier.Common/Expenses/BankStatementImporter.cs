using System;
using System.Collections.Generic;
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

        protected override IEnumerable<BankStatementRecord> ProcessRecords(IEnumerable<BankStatementRecord> records)
        {
            var counter = 1;
            foreach (var record in records)
            {
                yield return new BankStatementRecord
                {
                    ItemId = counter.ToString(),
                    Number = record.Number,
                    FirstBankCardNumber = record.FirstBankCardNumber,
                    TransactionTypeString = record.TransactionTypeString,
                    PostedAt = record.PostedAt,
                    Amount = record.Amount,
                    Description = record.Description
                };

                counter += 1;
            }
        }
    }
}
