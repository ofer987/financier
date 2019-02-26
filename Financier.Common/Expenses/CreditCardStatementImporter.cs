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
    // TODO: Trim values and set Valid function
    public class CreditCardStatementRecord
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

    public class CreditCardStatementImporter
    {
        public Statement Import(DateTime postedAt, Stream stream)
        {
            Card card = null;

            CreditCardStatementRecord[] records;
            using (var reader = new StreamReader(stream))
            using (var csv = new CsvReader(reader))
            {
                // Do nothing if record is faulty
                csv.Configuration.BadDataFound = (context) =>
                {
                    // TODO: log this to an error log
                    Console.WriteLine($"This line is faulty {context.Record.Join()}");
                };

                records = csv.GetRecords<CreditCardStatementRecord>().ToArray();
            }

            if (records.Length == 0)
            {
                // TODO: change to null-object pattern NulLStatement
                return null;
            }

            foreach (var record in records)
            {
                card = GetCard(record.CardNumber);
                var statement = GetStatement(postedAt, card);
                try
                {
                    CreateItem(record, statement.Id);
                }
                catch (DbUpdateException)
                {
                }
                catch (Exception exception)
                {
                    // TODO: Record error in logger
                    Console.WriteLine("Error creating item");
                    Console.WriteLine(exception);

                    // Continue to next record
                }
            }

            // reload the statement
            return FindOrCreateStatement(postedAt, card.Id);
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
        public Statement GetStatement(DateTime postedAt, Card card)
        {
            if (_statement != null)
            {
                return _statement;
            }

            return (_statement = FindOrCreateStatement(postedAt, card.Id));
        }

        public Statement FindOrCreateStatement(DateTime postedAt, Guid cardId)
        {
            using (var db = new Context())
            {
                var statement = db.Statements
                    .Include(stmt => stmt.Card)
                    .Include(stmt => stmt.Items)
                    .Where(stmt => stmt.CardId == cardId)
                    .Where(stmt => stmt.PostedAt == postedAt)
                    .FirstOrDefault();

                if (statement == null)
                {
                    statement = new Statement
                    {
                        Id = Guid.NewGuid(),
                        PostedAt = postedAt,
                        CardId = cardId,
                        Items = new List<Item>(),
                    };
                    db.Statements.Add(statement);
                    db.SaveChanges();

                    return statement;
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

        public Item CreateItem(CreditCardStatementRecord record, Guid statementId)
        {
            if (record.ItemId.Trim().IsNullOrEmpty())
            {
                throw new ArgumentException("cannot be blank or whitespace", nameof(record.ItemId));
            }

            using (var db = new Context())
            {
                var newItem = new Item
                {
                    Id = Guid.NewGuid(),
                    StatementId = statementId,
                    ItemId = record.ItemId.Trim(),
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

        public static Item[] GetItems()
        {
            using (var db = new Context())
            {
                return db.Items
                    .Include(item => item.ItemTags)
                    .Reject(item => item.ItemTags.Any())
                    .ToArray();
            }
        }
    }
}
