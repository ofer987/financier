using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using CsvHelper;
using CsvHelper.Configuration.Attributes;
using Financier.Common.Models.Expenses;

namespace Financier.Common
{
    public class StatementRecord
    {
        [Name("Item #")]
        public string ItemId { get; set; }

        [Name("Card #")]
        public string CardNumber { get; set; }

        [Name("Transaction Date")]
        public string TransactedAt { get; set; }

        [Name("Posting Date")]
        public string PostedAt { get; set; }

        [Name("Transaction Amount")]
        public string Amount { get; set; }

        [Name("Description")]
        public string Description { get; set; }

        public override string ToString()
        {
            return $"{nameof(ItemId)}: ({ItemId})\n{nameof(CardNumber)}: ({CardNumber})\n{nameof(TransactedAt)}: ({TransactedAt})\n{nameof(PostedAt)}: ({PostedAt})\n{nameof(Amount)}: ({Amount})\n{nameof(Description)}: ({Description})";
        }
    }

    public class StatementImporter
    {
        public static Statement Import(Guid statementId, DateTime postedAt, Stream stream)
        {
            using (var reader = new StreamReader(stream))
            using (var csv = new CsvReader(reader))
            {
                var records = csv.GetRecords<StatementRecord>();

                // Console.WriteLine(records.Count());
                var first = records.FirstOrDefault();
                try
                {
                    Console.WriteLine(first);
                    var card = FindOrCreateCard(first.CardNumber);
                    var statement = GetStatement(statementId, postedAt, card);
                    var item = CreateItem(first, statement);
                    statement.CardId = card.Id;
                }
                catch (Exception exception)
                {
                    Console.WriteLine("Error!");
                    Console.WriteLine(exception);
                    // Continue to next record
                    // TODO: Record error in logger
                }

                var rest = records.Skip(1);
                foreach (var record in rest)
                {
                    try
                    {
                        Console.WriteLine(record);
                        var card = FindOrCreateCard(first.CardNumber);
                        var statement = GetStatement(statementId, postedAt, card);
                        var item = CreateItem(first, statement);
                        statement.CardId = card.Id;
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine("Error!");
                        Console.WriteLine(exception);
                        // Continue to next record
                        // TODO: Record error in logger
                    }
                }
            }

            return statement;
        }

        // TODO: Rename
        private static Card card = null;
        public static Card GetCard(string cardNumber)
        {
            if (card != null)
            {
                return card;
            }

            return (card = FindOrCreateCard(cardNumber));
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
                    Card = card,
                    Items = new List<Item>()
                };
                if (card.Statements == null)
                {
                    Console.WriteLine($"NOW card.Statements is null");
                }
                else
                {
                    Console.WriteLine($"NOW Has {card.Statements.Count} statements");
                }
                var statement = card.Statements
                    .Where(stmt => stmt.Id == id)
                    .DefaultIfEmpty(null)
                    .First();

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
                        Number = cardNumber,
                        Statements = new List<Statement>()
                };
                var card = db.Cards
                    .DefaultIfEmpty(null)
                    .FirstOrDefault(cd => cd.Number == cardNumber);

                if (card == null)
                {
                    Console.WriteLine("Creating a new card");
                    card = newCard;
                    db.Cards.Add(newCard);
                    db.SaveChanges();
                }
                else
                {
                    Console.WriteLine("using existing card");
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
                TransactedAt = ToDateTime(record.TransactedAt),
                PostedAt = ToDateTime(record.PostedAt)
            };

            if (statement == null)
            {
                Console.WriteLine("statment is null");
            }
            statement.Items.Add(newItem);
            return newItem;
        }

        public static DateTime ToDateTime(string str)
        {
            return DateTime.ParseExact(str, "yyyyMMdd", null);
        }
    }
}
