using System;
using System.IO;
using System.Collections.Generic;

using Financier.Common.Expenses.Models;

namespace Financier.Common.Expenses
{
    public class BankStatementFile : StatementFile<BankStatementRecord>
    {
        public string AccountName { get; private set; }

        public BankStatementFile(string accountName, Stream stream, DateTime postedAt) : base(stream, postedAt)
        {
            AccountName = accountName;
        }

        public BankStatementFile(string accountName, FileInfo file) : base(file)
        {
            AccountName = accountName;
        }

        public BankStatementFile(string accountName, string path) : base(path)
        {
            AccountName = accountName;
        }

        protected override IEnumerable<BankStatementRecord> PostProcessedRecords(IEnumerable<BankStatementRecord> records)
        {
            var counter = 1;
            foreach (var record in records)
            {
                yield return new BankStatementRecord
                {
                    AccountName = AccountName,
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
