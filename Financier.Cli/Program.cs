using System;
using System.Collections.Generic;

using Financier.Common;

namespace Financier.Cli
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ExpensesContext.Environment = Environments.Dev;

            var postedAt = GetPostedAt(args);
            var stream = System.IO.File.OpenRead(GetStatementPath(args));

            new StatementImporter().Import(Guid.NewGuid(), postedAt, stream);
        }

        public static DateTime GetPostedAt(IReadOnlyList<string> args)
        {
            return DateTime.ParseExact(args[0], "yyyyMMdd", null);
        }

        public static string GetStatementPath(IReadOnlyList<string> args)
        {
            return args[1];
        }
    }
}
