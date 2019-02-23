using System;
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

        public string[] GetAll()
        {
            return new DirectoryInfo(Path)
                .GetFiles("*.csv")
                .Select(file => file.FullName)
                .ToArray();
        }

        public void FooBar()
        {
        }
    }
}

