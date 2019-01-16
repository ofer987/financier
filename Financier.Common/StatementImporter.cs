using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

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
                var records = csv.GetRecords<StatementRecord>().ToList();

                Console.WriteLine(records.Count);
                foreach (var record in records)
                {
                    try
                    {
                        Console.WriteLine(record);
                        var card = FindOrCreateCard(record.CardNumber);
                        var statement = GetStatement(statementId, postedAt, card);
                        CreateItem(record, statement);
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

            using (var db = new ExpensesContext())
            {
                return db.Statements
                    .Include(stmt => stmt.Card)
                    .ThenInclude(card => card.Statements)
                    .ThenInclude(stmt => stmt.Items)
                    .First(stmt => stmt.Id == statementId);
            }
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
                    CardId = card.Id,
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
                var statement = db.Statements
                    .Include(stmt => stmt.Items)
                    .Where(stmt => stmt.Id == id)
                    .FirstOrDefault();

                if (statement == null)
                {
                    card.Statements.Add(newStatement);
                    db.Statements.Add(newStatement);
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
                        Number = CleanCardNumber(cardNumber),
                        Statements = new List<Statement>()
                };
                var card = db.Cards
                    .Include(cd => cd.Statements)
                    .ThenInclude(stmt => stmt.Items)
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
                    Console.WriteLine($"There are ({db.Statements.Where(stmt => stmt.CardId == card.Id).Count()}) statements");
                    Console.WriteLine($"There are ({card.Statements.Count()}) card.Statements");
                }

                return card;
            }
        }

        public static Item CreateItem(StatementRecord record, Statement statement)
        {
            var newItem = new Item
            {
                Id = Guid.NewGuid(),
                StatementId = statement.Id,
                Description = record.Description,
                Amount = Convert.ToDecimal(record.Amount),
                TransactedAt = ToDateTime(record.TransactedAt),
                PostedAt = ToDateTime(record.PostedAt)
            };

            if (statement == null)
            {
                // NOTE: should never come here
                Console.WriteLine("statment is null");
            }
            using (var db = new ExpensesContext())
            {
                statement.Items.Add(newItem);
                db.Items.Add(newItem);
                db.SaveChanges();
            }
            return newItem;
        }

        public static DateTime ToDateTime(string str)
        {
            return DateTime.ParseExact(str, "yyyyMMdd", null);
        }

        public static string CleanCardNumber(string val)
        {
            return val;
        }
    }
}
