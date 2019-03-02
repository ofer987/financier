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
    public abstract class StatementImporter<T> where T : IStatementRecord
    {
        public void Import(DateTime postedAt, Stream stream)
        {
            foreach (var record in ParseStatement(stream))
            {
                var card = GetCard(record.Number);
                var statement = GetStatement(postedAt, card);
                try
                {
                    CreateItem(record, statement.Id);
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

        public T[] ParseStatement(Stream stream)
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

                return ProcessRecords(csv.GetRecords<T>()).ToArray();
            }
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
            cardNumber = CleanNumber(cardNumber);
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
                        Number = CleanNumber(cardNumber),
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

        public abstract Item CreateItem(T record, Guid statementId);

        public DateTime ToDateTime(string str)
        {
            return DateTime.ParseExact(str, "yyyyMMdd", null);
        }

        public string CleanNumber(string val)
        {
            var regex = new Regex(@"^\s*'?\s*(\w+)\s*'?\s*$");

            if (!regex.Match(val).Success || regex.Match(val).Groups.Count < 2)
            {
                throw new Exception($"Could not get card number from ({val})");
            }

            return regex.Match(val).Groups[1].Value;
        }

        protected virtual IEnumerable<T> ProcessRecords(IEnumerable<T> records)
        {
            return records;
        }
    }
}
