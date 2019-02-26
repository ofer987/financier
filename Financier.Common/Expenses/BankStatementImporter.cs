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
    public class BankStatementRecord
    {
        public enum TransactionTypes { Debit, Credit }

        [Name("Account")]
        public string AccountNumber { get; set; }

        [Name("First Bank Card")]
        public string FirstBankCardNumber { get; set; }

        [Name("Transaction Type")]
        public string TransactionTypeString { get; set; }

        [Name("Date Posted")]
        public string PostedAt { get; set; }

        [Name("Transaction Amount")]
        public string Amount { get; set; }

        [Name("Description")]
        public string Description { get; set; }

        public TransactionTypes TransactionType
        {
            get
            {
                switch ((TransactionTypeString ?? string.Empty).Trim().ToUpper())
                {
                    case "CREDIT":
                        return TransactionTypes.Credit;
                    case "DEBIT":
                    default:
                        return TransactionTypes.Debit;
                }
            }
        }

        public virtual bool IsValid()
        {
            return true;
        }

        public override int GetHashCode()
        {
            return AccountNumber.GetHashCode() 
                + FirstBankCardNumber.GetHashCode()
                + TransactionTypeString.GetHashCode()
                + PostedAt.GetHashCode()
                + Amount.GetHashCode()
                + Description.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var other = obj as BankStatementRecord;
            if (other == null)
            {
                return false;
            }

            if (AccountNumber != other.AccountNumber
                || FirstBankCardNumber != other.FirstBankCardNumber
                || TransactionTypeString != other.TransactionTypeString
                || PostedAt != other.PostedAt
                || Amount != other.Amount
                || Description != other.Description)
            {
                return false;
            }

            return true;
        }

        public override string ToString()
        {
            return $"{nameof(AccountNumber)}: ({AccountNumber})\n{nameof(FirstBankCardNumber)}: ({FirstBankCardNumber})\n{nameof(TransactionType)}: ({TransactionType})\n{nameof(PostedAt)}: ({PostedAt})\n{nameof(Amount)}: ({Amount})\n{nameof(Description)}: ({Description})";
        }
    }

    public class BankStatementImporter
    {
        public void Import(DateTime postedAt, Stream stream)
        {
            foreach (var record in ParseStatement(stream))
            {
                var card = GetCard(record.FirstBankCardNumber);
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

        public BankStatementRecord[] ParseStatement(Stream stream)
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

                return csv.GetRecords<BankStatementRecord>().ToArray();
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

        public Item CreateItem(BankStatementRecord record, Guid statementId)
        {
            if (!record.IsValid())
            {
                throw new ArgumentException("Record is not valid");
            }

            using (var db = new Context())
            {
                var newItem = new Item
                {
                    Id = Guid.NewGuid(),
                    StatementId = statementId,
                    ItemId = Guid.NewGuid().ToString(),
                    Description = record.Description,
                    // Credit values appear as increases to account, while
                    // debits appear
                    Amount = 0.00M - Convert.ToDecimal(record.Amount),
                    TransactedAt = ToDateTime(record.PostedAt),
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

        public string CleanNumber(string val)
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
