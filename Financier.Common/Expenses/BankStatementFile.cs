using System;
using System.IO;
using System.Collections.Generic;

using Financier.Common.Expenses.Models;

namespace Financier.Common.Expenses
{
    public class BankStatementFile : StatementFile<BankStatementRecord>
    {
        public BankStatementFile(string accountName, Stream stream, DateTime postedAt) : base(accountName, stream, postedAt)
        {
        }

        public BankStatementFile(string accountName, FileInfo file) : base(accountName, file)
        {
        }

        public BankStatementFile(string accountName, string path) : base(accountName, path)
        {
        }

        protected override IEnumerable<BankStatementRecord> PostProcessedRecords(string accountName, IEnumerable<BankStatementRecord> records)
        {
            var counter = 1;
            foreach (var record in records)
            {
                yield return new BankStatementRecord
                {
                    AccountName = accountName,
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
