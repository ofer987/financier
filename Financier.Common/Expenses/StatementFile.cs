using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

using CsvHelper;
using Financier.Common.Expenses.Models;
using Financier.Common.Extensions;

namespace Financier.Common.Expenses
{
    public abstract class StatementFile<T> where T : StatementRecord
    {
        private static Regex DateRegex = new Regex(@"\d{8}", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public Stream Stream { get; private set; }

        public DateTime PostedAt { get; private set; }

        protected StatementFile(FileInfo file)
        {
            Stream = file.OpenRead();
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
                    Console.WriteLine("Error creating item");
                    Console.WriteLine(exception);

                    // Continue to next record
                }
            }
        }

        public T[] GetRecords()
        {
            using (var reader = new StreamReader(Stream))
            using (var csv = new CsvReader(reader))
            {
                // Do nothing if record is faulty
                csv.Configuration.BadDataFound = (context) =>
                {
                    // TODO: log this to an error log
                    Console.WriteLine($"This line is faulty {context.Record.Join()}");
                };

                return PostProcessedRecords(csv.GetRecords<T>()).ToArray();
            }
        }

        protected virtual IEnumerable<T> PostProcessedRecords(IEnumerable<T> records)
        {
            return records;
        }
    }
}
