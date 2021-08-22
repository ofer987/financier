using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Financier.Cli
{
    public class AccountStatements
    {
        public string AccountName { get; private set; }
        public string FullPath { get; private set; }
        public FileInfo[] CsvFiles { get; private set; }

        private static Regex DateRegex = new Regex(@"\d{8}", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static IEnumerable<AccountStatements> GetAccountStatementsList(string path)
        {
            System.Console.WriteLine("1");
            var accounts = new DirectoryInfo(path).GetDirectories();
            System.Console.WriteLine("2");

            return accounts
                .Where(account => account.Exists)
                .Select(account => new AccountStatements(account.Name, account.FullName));
        }

        public void InitCsvFiles()
        {
            CsvFiles = new DirectoryInfo(FullPath)
                .GetFiles("*.csv", SearchOption.AllDirectories)
                .Where(IsStatement)
                .OrderBy(file => file.Name)
                .ToArray();
        }

        public bool IsStatement(FileInfo file)
        {
            return DateRegex.Match(file.Name).Success;
        }

        protected AccountStatements(string accountName, string path)
        {
            AccountName = accountName;
            FullPath = path;

            InitCsvFiles();
        }
    }
}
