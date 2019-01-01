using System;
using System.IO;
using System.Linq;

using CsvHelper;
using Financier.Common.Models.Expenses;

namespace Financier.Common
{
    public class StatementRecord
    {
        public string ItemId { get; set; }

        public string CardNumber { get; set; }

        public string TransactedAt { get; set; }

        public string PostedAt { get; set; }

        public string Amount { get; set; }

        public string Description { get; set; }
    }

    public class StatementImporter
    {
        public static void Import(Guid statementId, DateTime postedAt, string path)
        {
            using (var reader = new StreamReader(path))
            using (var csv = new CsvReader(reader))
            {
                var records = csv.GetRecords<StatementRecord>();

                var first = records.First();
                try
                {
                    var card = FindOrCreateCard(first.CardNumber);
                    var statement = GetStatement(statementId, postedAt, card);
                    var item = CreateItem(first, statement);
                    statement.CardId = card.Id;
                }
                catch (Exception)
                {
                    // Continue to next record
                    // TODO: Record error in logger
                }

                var rest = records.Skip(1);
                foreach (var record in rest)
                {
                    try
                    {
                        var card = FindOrCreateCard(first.CardNumber);
                        var statement = GetStatement(statementId, postedAt, card);
                        var item = CreateItem(first, statement);
                        statement.CardId = card.Id;
                    }
                    catch (Exception)
                    {
                        // Continue to next record
                        // TODO: Record error in logger
                    }
                }
            }
        }

        // TODO: Rename
        private static Statement statement = null;
        public static Statement GetStatement(Guid id, DateTime postedAt, Card card)
        {
            if (statement != null)
            {
                return statement;
            }

            return (statement = FindOrCreateStatement(id, postedAt, card));
        }

        public static Statement FindOrCreateStatement(Guid id, DateTime postedAt, Card card)
        {
            using (var db = new ExpensesContext())
            {
                var newStatement = new Statement
                {
                    Id = id,
                    PostedAt = postedAt,
                    Card = card
                };
                var statement = card.Statements
                    .DefaultIfEmpty(null)
                    .FirstOrDefault(stmt => stmt.Id == id);

                if (statement == null)
                {
                    card.Statements.Add(newStatement);
                    db.SaveChanges();
                    statement = newStatement;
                }

                return statement;
            }
        }

        public static Card FindOrCreateCard(string cardNumber)
        {
            using (var db = new ExpensesContext())
            {
                var newCard = new Card 
                {
                        Id = Guid.NewGuid(),
                        Number = cardNumber
                };
                var card = db.Cards
                    .DefaultIfEmpty(null)
                    .FirstOrDefault(cd => cd.Number == cardNumber);

                if (card == null)
                {
                    card = newCard;
                    db.Cards.Add(newCard);
                    db.SaveChanges();
                }

                return card;
            }
        }

        public static Item CreateItem(StatementRecord record, Statement statement)
        {
            var newItem = new Item
            {
                Id = Guid.NewGuid(),
                Statement = statement,
                Description = record.Description,
                Amount = Convert.ToDecimal(record.Amount),
                TransactedAt = Convert.ToDateTime(record.TransactedAt),
                PostedAt = Convert.ToDateTime(record.PostedAt)
            };

            statement.Items.Add(newItem);
            return newItem;
        }
    }
}
