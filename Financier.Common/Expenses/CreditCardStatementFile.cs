using System;
using System.IO;

using Financier.Common.Expenses.Models;

namespace Financier.Common.Expenses
{
    public class CreditCardStatementFile : StatementFile<CreditCardStatementRecord>
    {
        public CreditCardStatementFile(Stream stream, DateTime postedAt) : base(stream, postedAt)
        {
        }

        public CreditCardStatementFile(FileInfo file) : base(file)
        {
        }

        public CreditCardStatementFile(string path) : base(path)
        {
        }
    }
}
