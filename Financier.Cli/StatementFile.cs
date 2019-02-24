using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using Financier.Common.Expenses;
using Financier.Common.Expenses.Models;

namespace Financier.Cli
{
    public class StatementFile
    {
        private static Regex DateRegex = new Regex(@"\d{8}", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static FileInfo[] GetCsvFiles(string path)
        {
            return new DirectoryInfo(path)
                .GetFiles("*.csv", SearchOption.AllDirectories)
                .Where(IsStatement)
                .OrderBy(file => file.Name)
                .ToArray();
        }

        public static bool IsStatement(FileInfo file)
        {
            return DateRegex.Match(file.Name).Success;
        }

        public FileInfo File { get; private set; }

        public StatementFile(FileInfo file)
        {
            File = file;
        }

        public DateTime GetPostedAt()
        {
            var str = DateRegex.Match(File.Name).Value;

            return DateTime.ParseExact(str, "yyyyMMdd", null);
        }

        public FileStream GetFileStream()
        {
            return File.OpenRead();
        }

        public Statement Import()
        {
            return new StatementImporter().Import(GetPostedAt(), GetFileStream());
        }
    }
}
