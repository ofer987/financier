using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

using CsvHelper;
using CsvHelper.Configuration.Attributes;
using Financier.Common.Expenses.Models;
using Financier.Common.Extensions;

namespace Financier.Common.Expenses
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
        public Statement Import(Guid statementId, DateTime postedAt, Stream stream)
        {
            using (var reader = new StreamReader(stream))
            using (var csv = new CsvReader(reader))
            {
                // Do nothing if record is faulty
                csv.Configuration.BadDataFound = (context) =>
                {
                    // TODO: log this to an error log
                    Console.WriteLine($"This line is faulty {context.Record.Join()}");
                };

                var records = csv.GetRecords<StatementRecord>().ToList();

                foreach (var record in records)
                {
                    // Console.WriteLine(record);
                    var card = GetCard(record.CardNumber);
                    var statement = GetStatement(statementId, postedAt, card);
                    try
                    {
                        var item = CreateItem(record, statement);
                        // FindOrCreateTags(string.Empty);
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

            using (var db = new Context())
            {
                return db.Statements
                    .Include(stmt => stmt.Card)
                    .ThenInclude(card => card.Statements)
                    .ThenInclude(stmt => stmt.Items)
                    .FirstOrDefault(stmt => stmt.Id == statementId);
            }
        }

        private Card _card = null;
        public Card GetCard(string cardNumber)
        {
            if (_card != null)
            {
                return _card;
            }

            return (_card = FindOrCreateCard(cardNumber));
        }

        private Statement _statement = null;
        public Statement GetStatement(Guid id, DateTime postedAt, Card card)
        {
            if (_statement != null)
            {
                return _statement;
            }

            return (_statement = FindOrCreateStatement(id, postedAt, card.Id));
        }

        public Statement FindOrCreateStatement(Guid id, DateTime postedAt, Guid cardId)
        {
            using (var db = new Context())
            {
                var statement = db.Statements
                    .Include(stmt => stmt.Items)
                    .Where(stmt => stmt.CardId == cardId)
                    .ToArray()
                    .Where(stmt => stmt.PostedAt.Year == postedAt.Year && stmt.PostedAt.Month == postedAt.Month)
                    .FirstOrDefault();

                if (statement == null)
                {
                    var newStatement = new Statement
                    {
                        Id = id,
                        PostedAt = postedAt,
                        CardId = cardId,
                        Items = new List<Item>(),
                    };
                    db.Statements.Add(newStatement);
                    db.SaveChanges();

                    statement = db.Statements
                        .Include(stmt => stmt.Items)
                        .First(st => st.Id == id);
                }

                return statement;
            }
        }

        public Card FindOrCreateCard(string cardNumber)
        {
            cardNumber = CleanCardNumber(cardNumber);
            using (var db = new Context())
            {
                var card = db.Cards
                    .Include(cd => cd.Statements)
                    .ThenInclude(stmt => stmt.Items)
                    .FirstOrDefault(cd => cd.Number == cardNumber);

                if (card == null)
                {
                    var newCard = new Card
                    {
                        Id = Guid.NewGuid(),
                        Number = CleanCardNumber(cardNumber),
                        Statements = new List<Statement>()
                    };
                    db.Cards.Add(newCard);
                    db.SaveChanges();

                    // Reload the card
                    card = db.Cards
                        .Include(cd => cd.Statements)
                        .ThenInclude(stmt => stmt.Items)
                        .FirstOrDefault(cd => cd.Number == cardNumber);
                }

                return card;
            }
        }

        public Item CreateItem(StatementRecord record, Statement statement)
        {
            using (var db = new Context())
            {
                var newItem = new Item
                {
                    Id = Guid.NewGuid(),
                    // Statement = statement,
                    StatementId = statement.Id,
                    Description = record.Description,
                    Amount = Convert.ToDecimal(record.Amount),
                    TransactedAt = ToDateTime(record.TransactedAt),
                    PostedAt = ToDateTime(record.PostedAt)
                };
                db.Items.Add(newItem);
                db.SaveChanges();

                return newItem;
            }
        }

        public DateTime ToDateTime(string str)
        {
            return DateTime.ParseExact(str, "yyyyMMdd", null);
        }

        public string CleanCardNumber(string val)
        {
            var regex = new Regex(@"^\s*'?\s*(\w+)\s*'?\s*$");

            if (!regex.Match(val).Success || regex.Match(val).Groups.Count < 2)
            {
                throw new Exception($"Could not get card number from ({val})");
            }

            return regex.Match(val).Groups[1].Value;
        }
    }
}
