using System;
using System.IO;
using System.Linq;

using CsvHelper;

namespace Financier.Common
{
    public class StatementRecord
    {
        public string ItemId { get; set; }

        public string CardId { get; set; }

        public string TransactionDate { get; set; }

        public string PostingDate { get; set; }

        public string Amount { get; set; }

        public string Description { get; set; }
    }

    public class StatementImporter
    {
        public static void Import(string statement, string path)
        {
            var file = File.OpenRead(path);

            var statement2 = CreateNewStatement(statement)

            using (var reader = new StreamReader(path))
            using (var csv = new CsvReader(reader))
            {
                var records = csv.GetRecords<StatementRecord>();

                var first = records.First();

                Statement statement3;
                {
                    var item = CreateItem(first);
                    statement3 = CreateStatement(first, statement);
                    item.StatementId = statement3.Id;
                    var card = FindOrCreateCard(first);
                    statement3.CardId = card.Id;
                }

                var rest = records.Skip(1);
                foreach (var record in rest)
                {
                    var item = CreateItem(first);
                    item.StatementId = statement3.Id;
                }
            }
        }
    }
}
