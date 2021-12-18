using System;
using System.Collections.Generic;

using System.IO;
using System.Text.RegularExpressions;

using Financier.Common.Expenses.Models;

namespace Financier.Common.Expenses
{
    public class CreditCardStatementFile : StatementFile<CreditCardStatementRecord>
    {
        public string AccountName { get; private set; }
        public string Number { get; private set; }

        public CreditCardStatementFile(string accountName, Stream stream, DateTime postedAt) : base(stream, postedAt)
        {
            AccountName = accountName;
        }

        public CreditCardStatementFile(string accountName, FileInfo file) : base(file)
        {
            AccountName = accountName;
            var creditCardRegex = new Regex(@"(\d+)");
            Number = creditCardRegex.Match(file.FullName).Name;
        }

        public CreditCardStatementFile(string accountName, string path) : base(path)
        {
        }

        protected override IEnumerable<CreditCardStatementRecord> PostProcessedRecords(IEnumerable<CreditCardStatementRecord> records)
        {
            foreach (var record in records)
            {
                yield return new CreditCardStatementRecord
                {
                    AccountName = AccountName,
                    ItemId = record.ItemId,
                    Number = Number,
                    PostedAt = record.PostedAt,
                    TransactedAt = record.TransactedAt,
                    Amount = record.Amount,
                    Description = record.Description
                };
            }
        }
    }
}
