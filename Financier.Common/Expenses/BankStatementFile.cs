using System;
using System.IO;
using System.Collections.Generic;

using Financier.Common.Expenses.Models;

namespace Financier.Common.Expenses
{
    public class BankStatementFile : StatementFile<BankStatementRecord>
    {
        public BankStatementFile(Stream stream, DateTime postedAt) : base(stream, postedAt)
        {
        }

        public BankStatementFile(FileInfo file) : base(file)
        {
        }

        public BankStatementFile(string path) : base(path)
        {
        }

        protected override IEnumerable<BankStatementRecord> PostProcessedRecords(IEnumerable<BankStatementRecord> records)
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
