using System;
using System.IO;

using Financier.Common.Expenses.Models;

namespace Financier.Common.Expenses
{
    public class CreditCardStatementFile : StatementFile<CreditCardStatementRecord>
    {
        public CreditCardStatementFile(string accountName, Stream stream, DateTime postedAt) : base(accountName, stream, postedAt)
        {
        }

        public CreditCardStatementFile(string accountName, FileInfo file) : base(accountName, file)
        {
        }

        public CreditCardStatementFile(string accountName, string path) : base(accountName, path)
        {
        }
    }
}
