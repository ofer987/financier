using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Financier.Cli
{
    public class Statements
    {
        public string AccountName { get; private set; }
        public FileInfo[] CsvFiles { get; private set; }

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

        public Statements(string path)
        {
            AccountName = Path.GetDirectoryName(path)
                .Split(Path.PathSeparator)
                .Last();
            CsvFiles = GetCsvFiles(path);
        }
    }
}
