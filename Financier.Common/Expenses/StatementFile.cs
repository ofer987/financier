using System;
using System.IO;
using System.Linq;
using System.Globalization;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

using CsvHelper;
using Financier.Common.Expenses.Models;

namespace Financier.Common.Expenses
{
    public abstract class StatementFile<T> where T : StatementRecord
    {
        private static Regex DateRegex = new Regex(@"\d{8}", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public Stream Stream { get; private set; }
        public DateTime PostedAt { get; private set; }
        public string Path { get; private set; }

        protected StatementFile(FileInfo file)
        {
            Stream = file.OpenRead();
            Path = file.FullName;
            PostedAt = DateTime.ParseExact(DateRegex.Match(file.Name).Value, "yyyyMMdd", null);
        }

        protected StatementFile(string path) : this(new FileInfo(path))
        {
        }

        protected StatementFile(Stream stream, DateTime postedAt)
        {
            Stream = stream;
            PostedAt = postedAt;
        }

        public void Import()
        {
            var records = GetRecords();
            if (records.Length == 0)
            {
                return;
            }

            var card = records[0].GetCard();
            var statement = records[0].GetStatement(PostedAt);
            Console.WriteLine(this.Path);
            Console.WriteLine("1");

            foreach (var record in records)
            {
                try
                {
                    record.CreateItem(statement.Id);
                }
                catch (DbUpdateException)
                {
                    // Record already exists, so ignore this error
                }
                catch (Exception exception)
                {
                    // TODO: Record error in logger
                    Console.WriteLine($"Error creating item for file at ({Path})");
                    Console.WriteLine(exception);

                    // Continue to next record
                }

                Console.WriteLine(2);
            }
        }

        public T[] GetRecords()
        {
            using (var reader = new StreamReader(Stream))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                return PostProcessedRecords(csv.GetRecords<T>()).ToArray();
            }
        }

        protected virtual IEnumerable<T> PostProcessedRecords(IEnumerable<T> records)
        {
            return records;
        }
    }
}
