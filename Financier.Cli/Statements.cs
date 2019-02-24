using System.IO;
using System.Linq;

namespace Financier.Cli
{
    public class Statements
    {
        public string Path { get; private set; }

        public Statements(string path)
        {
            Path = path;
        }

        public FileInfo[] GetAll()
        {
            return new DirectoryInfo(Path)
                .GetFiles("*.csv", SearchOption.AllDirectories)
                .OrderBy(file => file.Name)
                .ToArray();
        }

        public void FooBar()
        {
        }
    }
}

