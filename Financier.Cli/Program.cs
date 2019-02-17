using System;
using System.Collections.Generic;

using Financier.Common;
using Financier.Common.Expenses;

namespace Financier.Cli
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Context.Environment = Environments.Dev;

            var postedAt = GetPostedAt(args);
            var stream = System.IO.File.OpenRead(GetStatementPath(args));

            var importer = new StatementImporter();
            var statement = importer.Import(Guid.NewGuid(), postedAt, stream);

            foreach (var item in statement.Items)
            {
                Console.WriteLine($"Input Tags for {item}");
                // importer.FindOrCreateTags(Console.ReadLine());
            }
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
